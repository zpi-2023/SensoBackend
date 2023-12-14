using FluentValidation;
using Hangfire;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Application.RemindersScheduler;
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
                "(@(annually|yearly|monthly|weekly|daily|hourly|reboot))|(@every (\\d+(ns|us|µs|ms|s|m|h))+)|((((\\d+,)+\\d+|(\\d+(\\/|-)\\d+)|(\\*\\/\\d+)|\\d+|\\*) ?){5,7})"
            )
            .WithMessage("Cron expression is invalid");
    }
}

[UsedImplicitly]
public sealed class CreateReminderHandler(AppDbContext context, IMediator mediator)
    : IRequestHandler<CreateReminderRequest, ReminderDto>
{
    public async Task<ReminderDto> Handle(CreateReminderRequest request, CancellationToken ct)
    {
        bool validProfileExists = await context
            .Profiles
            .Where(p => p.SeniorId == request.SeniorId && p.AccountId == request.AccountId)
            .AnyAsync(ct);

        if (!validProfileExists)
        {
            throw new SeniorReminderAccessDeniedException(request.SeniorId);
        }

        Medication medication;

        var medicationsFromDb = await context
            .Medications
            .Where(
                m =>
                    m.Name == request.Dto.MedicationName
                    && m.AmountInPackage == request.Dto.MedicationAmountInPackage
                    && m.AmountUnit == request.Dto.AmountUnit
            )
            .ToListAsync(ct);

        if (medicationsFromDb.Count == 0)
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

        await context.Reminders.AddAsync(reminder, ct);
        await context.SaveChangesAsync(ct);

        if (reminder.Cron is not null)
        {
            RecurringJob.AddOrUpdate(
                reminder.Id.ToString(),
                () => CreateReminderAlert(reminder.Id, reminder.SeniorId),
                reminder.Cron
            );
        }

        return ReminderUtils.AdaptToDto(context, reminder).Result;
    }

    public async Task CreateReminderAlert(int reminderId, int seniorId)
    {
        await mediator.Send(
            new CreateReminderAlertRequest { ReminderId = reminderId, SeniorId = seniorId }
        );
    }
}
