using SensoBackend.Domain.Entities;

namespace SensoBackend.Tests.Application.Modules.Medication.Utils;

public static class ReminderTestUtils
{
    public static Reminder GetFakeReminder() =>
        new Reminder
        {
            Id = default,
            SeniorId = 2137,
            MedicationId = 2137,
            IsActive = true,
            AmountPerIntake = 1,
            AmountOwned = 3,
            Cron = "1 1 1 * * *",
            Description = "Description"
        };
}
