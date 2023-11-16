using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
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

    public static async Task<LeaderboardEntry> SetupLeaderboardEntry(
        this AppDbContext context,
        Game game,
        Account account,
        int score
    )
    {
        var leaderboardEntry = new LeaderboardEntry
        {
            Id = default,
            AccountId = account.Id,
            Game = game,
            Score = score
        };
        await context.LeaderboardEntries.AddAsync(leaderboardEntry);
        await context.SaveChangesAsync();
        return leaderboardEntry;
    }

    public static async Task<LeaderboardEntry> SetupLeaderboardEntry(
        this AppDbContext context,
        Game game,
        int score
    )
    {
        var account = await context.SetUpAccount();
        return await context.SetupLeaderboardEntry(game, account, score);
    }
}
