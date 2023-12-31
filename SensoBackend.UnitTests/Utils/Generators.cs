using Bogus;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Dashboard.Contracts;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;

namespace SensoBackend.UnitTests.Utils;

public static class Generators
{
    static Generators()
    {
        Faker.DefaultStrictMode = true;
        Randomizer.Seed = new Random(0);
    }

    public static readonly Faker<AccountDto> AccountDto = new Faker<AccountDto>()
        .RuleFor(u => u.Id, f => f.IndexVariable++)
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Password, f => f.Internet.Password())
        .RuleFor(u => u.Active, _ => true)
        .RuleFor(u => u.Verified, _ => true)
        .RuleFor(u => u.PhoneNumber, f => f.Random.ReplaceNumbers("#########"))
        .RuleFor(u => u.DisplayName, f => f.Name.FirstName())
        .RuleFor(u => u.CreatedAt, f => f.Date.PastOffset())
        .RuleFor(u => u.LastLoginAt, f => f.Date.PastOffset())
        .RuleFor(u => u.LastPasswordChangeAt, f => f.Date.PastOffset())
        .RuleFor(u => u.Role, _ => Role.Member);

    public static readonly Faker<Account> Account = new Faker<Account>()
        .RuleFor(u => u.Id, _ => default)
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Password, f => f.Random.String(72))
        .RuleFor(u => u.Active, _ => true)
        .RuleFor(u => u.Verified, _ => true)
        .RuleFor(u => u.PhoneNumber, _ => null)
        .RuleFor(u => u.DisplayName, f => f.Name.FirstName())
        .RuleFor(u => u.CreatedAt, f => f.Date.PastOffset())
        .RuleFor(u => u.LastLoginAt, f => f.Date.PastOffset())
        .RuleFor(u => u.LastPasswordChangeAt, f => f.Date.PastOffset())
        .RuleFor(u => u.Role, _ => Role.Member);

    public static readonly Faker<GetAccountByCredentialsDto> GetAccountByCredentialsDto =
        new Faker<GetAccountByCredentialsDto>()
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => f.Internet.Password());

    public static readonly Faker<CreateAccountDto> CreateAccountDto = new Faker<CreateAccountDto>()
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Password, f => f.Internet.Password())
        .RuleFor(u => u.PhoneNumber, f => f.Random.ReplaceNumbers("#########"))
        .RuleFor(u => u.DisplayName, f => f.Random.Word());

    public static readonly Faker<DashboardDto> DashboardDto = new Faker<DashboardDto>().RuleFor(
        d => d.Gadgets,
        f =>
            Enumerable
                .Range(0, f.Random.Int(0, 6))
                .Select(_ => f.PickRandom<Gadget>().ToString("f"))
                .ToList()
    );

    public static readonly Faker<EditCaretakerProfileDto> EditCaretakerProfileDto =
        new Faker<EditCaretakerProfileDto>().RuleFor(u => u.SeniorAlias, f => f.Name.FirstName());

    public static readonly Faker<CreateIntakeDto> CreateIntakeDto = new Faker<CreateIntakeDto>()
        .RuleFor(i => i.TakenAt, f => f.Date.PastOffset())
        .RuleFor(i => i.AmountTaken, f => f.Random.Number() + 1);

    public static readonly Faker<CreateReminderDto> CreateReminderDto =
        new Faker<CreateReminderDto>()
            .RuleFor(r => r.MedicationName, f => f.Random.String())
            .RuleFor(r => r.MedicationAmountInPackage, f => f.Random.Number() + 1)
            .RuleFor(r => r.AmountPerIntake, f => f.Random.Number() + 1)
            .RuleFor(r => r.AmountOwned, f => f.Random.Number())
            .RuleFor(r => r.AmountUnit, _ => "g")
            .RuleFor(r => r.Cron, _ => "1 1 1 * *")
            .RuleFor(r => r.Description, f => f.Random.String());
    public static readonly Faker<UpdateReminderDto> UpdateReminderDto =
        new Faker<UpdateReminderDto>()
            .RuleFor(r => r.AmountPerIntake, _ => 2137)
            .RuleFor(r => r.AmountOwned, f => f.Random.Number())
            .RuleFor(r => r.Cron, _ => "1 1 1 * *")
            .RuleFor(r => r.Description, f => f.Random.String());
}
