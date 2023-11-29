using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications.Utils;

public static class ReminderUtils
{
    /// <summary>
    /// Checks if requested reminder exists and if account with a given id has a profile that allows it to manage this reminder
    /// </summary>
    /// <param name="context">Db context</param>
    /// <param name="accountId">Id of an account that sent the request</param>
    /// <param name="reminderId">Id of a reminder to be managed</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns> <see cref="Reminder"/> associated with a given reminderId</returns>
    /// <exception cref="ReminderNotFoundException">When reminder was not found - 404</exception>
    /// <exception cref="ReminderAccessDeniedException">
    /// When account with a given Id does not have a profile required to manage reminder - 403
    /// </exception>
    public static async Task<Reminder> CheckReminderAndProfile(
        AppDbContext context,
        int accountId,
        int reminderId,
        CancellationToken ct
    )
    {
        var reminder =
            await context.Reminders.FindAsync(reminderId, ct)
            ?? throw new ReminderNotFoundException(reminderId);

        var neededProfile =
            await context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == accountId && p.SeniorId == reminder.SeniorId
                ) ?? throw new ReminderAccessDeniedException(reminderId);

        return reminder;
    }

    public static async Task<ReminderDto> AdaptToDto(AppDbContext context, Reminder reminder)
    {
        var medication = await context.Medications.FindAsync(reminder.MedicationId);

        return new ReminderDto
        {
            Id = reminder.Id,
            SeniorId = reminder.SeniorId,
            MedicationName = medication!.Name,
            MedicationAmountInPackage = medication.AmountInPackage,
            IsActive = reminder.IsActive,
            AmountPerIntake = reminder.AmountPerIntake,
            AmountOwned = reminder.AmountOwned,
            AmountUnit = medication.AmountUnit,
            Cron = reminder.Cron,
            Description = reminder.Description
        };
    }

    public static async Task<IntakeDto> AdaptToDto(AppDbContext context, IntakeRecord intake)
    {
        var reminder = await context.Reminders.FindAsync(intake.ReminderId);
        var medication = await context.Medications.FindAsync(reminder!.MedicationId);

        return new IntakeDto
        {
            Id = intake.Id,
            ReminderId = intake.ReminderId,
            MedicationName = medication!.Name,
            TakenAt = intake.TakenAt,
            AmountTaken = intake.AmountTaken,
            AmountUnit = medication.AmountUnit
        };
    }
}
