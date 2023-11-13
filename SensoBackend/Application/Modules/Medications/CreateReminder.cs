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
        RuleFor(r => r.SeniorId)
            .NotEmpty()
            .WithMessage("SeniorId cannot be empty")
            .GreaterThan(0)
            .WithMessage("SeniorId has to be greater than 0");
        RuleFor(r => r.AccountId)
            .NotEmpty()
            .WithMessage("AccountId cannot be empty")
            .GreaterThan(0)
            .WithMessage("AccountId has to be greater than 0");
        RuleFor(r => r.Dto.MeicationName).NotEmpty().WithMessage("Medication name is empty");
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
        //TODO: I suck at regex but might use one for cron
        //RuleFor(r => r.Dto.Cron).Matches(...).WithMessage("Cron expression is invalid");
    }
}

[UsedImplicitly]
public sealed class CreateReminderHandler : IRequestHandler<CreateReminderRequest, ReminderDto>
{
    private readonly AppDbContext _context;
    private readonly ILogger<CreateReminderHandler> _logger;

    public CreateReminderHandler(AppDbContext context, ILogger<CreateReminderHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ReminderDto> Handle(
        CreateReminderRequest request,
        CancellationToken cancellationToken
    )
    {
        bool validProfileExists = await _context.Profiles
            .Where(p => p.SeniorId == request.SeniorId && p.AccountId == request.AccountId)
            .AnyAsync();

        if (!validProfileExists)
        {
            throw new SeniorReminderAccessDeniedException(request.SeniorId);
        }

        Medication medication;

        var medicationsFromDb = await _context.Medications
            .Where(
                m =>
                    m.Name == request.Dto.MeicationName
                    && m.AmountInPackage == request.Dto.MedicationAmountInPackage
                    && m.AmountUnit == request.Dto.AmountUnit
            )
            .ToListAsync();

        if (!medicationsFromDb.Any())
        {
            _logger.LogInformation("-----> Creating new Medication");

            var medicationDto = new MedicationDto
            {
                Name = request.Dto.MeicationName,
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

        await _context.Reminders.AddAsync(reminder);
        await _context.SaveChangesAsync();

        return ReminderUtils.AdaptToDto(_context, reminder).Result;
    }
}
