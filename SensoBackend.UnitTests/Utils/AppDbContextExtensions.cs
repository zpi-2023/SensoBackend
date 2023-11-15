using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.UnitTests.Utils;

internal static class AppDbContextExtensions
{
    public static async Task<Account> SetUpAccount(this AppDbContext context)
    {
        var account = Generators.Account.Generate();
        await context.Accounts.AddAsync(account);
        await context.SaveChangesAsync();
        return account;
    }

    public static async Task<Profile> SetUpSeniorProfile(this AppDbContext context, Account account)
    {
        var profile = new Profile
        {
            Id = default,
            AccountId = account.Id,
            SeniorId = account.Id,
            Alias = account.DisplayName
        };
        await context.Profiles.AddAsync(profile);
        await context.SaveChangesAsync();
        return profile;
    }

    public static async Task<Profile> SetUpCaretakerProfile(
        this AppDbContext context,
        Account caretakerAccount,
        Account seniorAccount
    )
    {
        var profile = new Profile
        {
            Id = default,
            AccountId = caretakerAccount.Id,
            SeniorId = seniorAccount.Id,
            Alias = seniorAccount.DisplayName
        };
        await context.Profiles.AddAsync(profile);
        await context.SaveChangesAsync();
        return profile;
    }

    public static async Task<Note> SetUpNote(
        this AppDbContext context,
        Account account,
        bool isPrivate = true
    )
    {
        if (
            !await context.Profiles.AnyAsync(
                p => p.AccountId == account.Id && p.SeniorId == account.Id
            )
        )
        {
            throw new ArgumentException("Account must have a senior profile", nameof(account));
        }

        var note = new Note
        {
            Id = default,
            AccountId = account.Id,
            Content = "My note content",
            CreatedAt = new DateTimeOffset(2006, 10, 13, 10, 11, 15, TimeSpan.Zero),
            IsPrivate = isPrivate,
            Title = "My note title"
        };
        await context.Notes.AddAsync(note);
        await context.SaveChangesAsync();
        return note;
    }

    public static async Task<Medication> SetUpMedication(this AppDbContext context)
    {
        var medication = new Medication
        {
            Id = default,
            Name = "medication",
            AmountInPackage = default,
            AmountUnit = "g"
        };

        await context.Medications.AddAsync(medication);
        await context.SaveChangesAsync();
        return medication;
    }

    public static async Task<Reminder> SetUpReminder(
        this AppDbContext context,
        Account userAccount,
        Account seniorAccount,
        Medication medication
    )
    {
        if (
            !await context.Profiles.AnyAsync(
                p => p.AccountId == userAccount.Id && p.SeniorId == seniorAccount.Id
            )
        )
        {
            throw new ArgumentException(
                "Account must have a profile related to the given senior",
                nameof(seniorAccount)
            );
        }

        var reminder = new Reminder
        {
            Id = default,
            SeniorId = seniorAccount.Id,
            MedicationId = default,
            IsActive = true,
            AmountPerIntake = default,
            AmountOwned = default,
            Cron = "1 1 1 * * *",
            Description = "Description"
        };
        await context.Reminders.AddAsync(reminder);
        await context.SaveChangesAsync();

        return reminder;
    }
}
