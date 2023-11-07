using Bogus;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Dashboard.Contracts;
using SensoBackend.Domain.Entities;

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
        .RuleFor(u => u.RoleId, _ => 1);

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
        .RuleFor(u => u.RoleId, _ => Role.Member.Id)
        .RuleFor(u => u.Role, _ => null);

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
                .Select(_ => Gadget.List[f.Random.Int(0, Gadget.List.Count - 1)].Name)
                .ToList()
    );
}
