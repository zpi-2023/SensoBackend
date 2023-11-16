using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record CreateReminderRequest : IRequest<ReminderDto>
{
    public required int AccountId { get; init; }

    public required int SeniorId { get; init; }

    public required CreateReminderDto Dto { get; init; }
}

[UsedImplicitly]
public sealed class CreateReminderValidator : AbstractValidator<CreateReminderRequest>
{
    public CreateReminderValidator()
    {
        RuleFor(r => r.Dto.MedicationName).NotEmpty().WithMessage("Medication name is empty");
        RuleFor(r => r.Dto.AmountPerIntake).NotEmpty().WithMessage("Amount per intake is empty");
        RuleFor(r => r.Dto.MedicationAmountInPackage)
            .GreaterThan(0)
            .WithMessage("Medication amount in package is 0 or less");
        RuleFor(r => r.Dto.AmountOwned)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Medication amount owned is negative");
        RuleFor(r => r.Dto.AmountPerIntake)
            .GreaterThan(0)
            .WithMessage("Medication amount per intake is 0 or less");
        RuleFor(r => r.Dto.Cron)
            .Matches(
                "^\\s*($|#|\\w+\\s*=|(\\?|\\*|(?:[0-5]?\\d)(?:(?:-|\\/|\\,)(?:[0-5]?\\d))?(?:,(?:[0-5]?\\d)(?:(?:-|\\/|\\,)(?:[0-5]?\\d))?)*)\\s+(\\?|\\*|(?:[0-5]?\\d)(?:(?:-|\\/|\\,)(?:[0-5]?\\d))?(?:,(?:[0-5]?\\d)(?:(?:-|\\/|\\,)(?:[0-5]?\\d))?)*)\\s+(\\?|\\*|(?:[01]?\\d|2[0-3])(?:(?:-|\\/|\\,)(?:[01]?\\d|2[0-3]))?(?:,(?:[01]?\\d|2[0-3])(?:(?:-|\\/|\\,)(?:[01]?\\d|2[0-3]))?)*)\\s+(\\?|\\*|(?:0?[1-9]|[12]\\d|3[01])(?:(?:-|\\/|\\,)(?:0?[1-9]|[12]\\d|3[01]))?(?:,(?:0?[1-9]|[12]\\d|3[01])(?:(?:-|\\/|\\,)(?:0?[1-9]|[12]\\d|3[01]))?)*)\\s+(\\?|\\*|(?:[1-9]|1[012])(?:(?:-|\\/|\\,)(?:[1-9]|1[012]))?(?:L|W)?(?:,(?:[1-9]|1[012])(?:(?:-|\\/|\\,)(?:[1-9]|1[012]))?(?:L|W)?)*|\\?|\\*|(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)(?:(?:-)(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC))?(?:,(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)(?:(?:-)(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC))?)*)\\s+(\\?|\\*|(?:[0-6])(?:(?:-|\\/|\\,|#)(?:[0-6]))?(?:L)?(?:,(?:[0-6])(?:(?:-|\\/|\\,|#)(?:[0-6]))?(?:L)?)*|\\?|\\*|(?:MON|TUE|WED|THU|FRI|SAT|SUN)(?:(?:-)(?:MON|TUE|WED|THU|FRI|SAT|SUN))?(?:,(?:MON|TUE|WED|THU|FRI|SAT|SUN)(?:(?:-)(?:MON|TUE|WED|THU|FRI|SAT|SUN))?)*)(|\\s)+(\\?|\\*|(?:|\\d{4})(?:(?:-|\\/|\\,)(?:|\\d{4}))?(?:,(?:|\\d{4})(?:(?:-|\\/|\\,)(?:|\\d{4}))?)*))$"
            )
            .WithMessage("Cron expression is invalid");
    }
}

[UsedImplicitly]
public sealed class CreateReminderHandler : IRequestHandler<CreateReminderRequest, ReminderDto>
{
    private readonly AppDbContext _context;

    public CreateReminderHandler(AppDbContext context) => _context = context;

    public async Task<ReminderDto> Handle(CreateReminderRequest request, CancellationToken ct)
    {
        bool validProfileExists = await _context.Profiles
            .Where(p => p.SeniorId == request.SeniorId && p.AccountId == request.AccountId)
            .AnyAsync(ct);

        if (!validProfileExists)
        {
            throw new SeniorReminderAccessDeniedException(request.SeniorId);
        }

        Medication medication;

        var medicationsFromDb = await _context.Medications
            .Where(
                m =>
                    m.Name == request.Dto.MedicationName
                    && m.AmountInPackage == request.Dto.MedicationAmountInPackage
                    && m.AmountUnit == request.Dto.AmountUnit
            )
            .ToListAsync(ct);

        if (!medicationsFromDb.Any())
        {
            var medicationDto = new MedicationDto
            {
                Name = request.Dto.MedicationName,
                AmountInPackage = request.Dto.MedicationAmountInPackage,
                AmountUnit = request.Dto.AmountUnit
            };

            medication = medicationDto.Adapt<Medication>();
        }
        else
        {
            medication = medicationsFromDb.First()!;
        }

        //add reminder
        var reminder = request.Dto.Adapt<Reminder>();
        reminder.SeniorId = request.SeniorId;
        reminder.Medication = medication;
        reminder.IsActive = true;

        await _context.Reminders.AddAsync(reminder, ct);
        await _context.SaveChangesAsync(ct);

        return ReminderUtils.AdaptToDto(_context, reminder).Result;
    }
}
