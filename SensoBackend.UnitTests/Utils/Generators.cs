using Bogus;
using SensoBackend.Application.Modules.Accounts.Contracts;

namespace SensoBackend.Tests.Utils;

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
        .RuleFor(u => u.DisplayName, f => f.Name.FullName())
        .RuleFor(u => u.CreatedAt, f => f.Date.PastOffset())
        .RuleFor(u => u.LastLoginAt, f => f.Date.PastOffset())
        .RuleFor(u => u.LastPasswordChangeAt, f => f.Date.PastOffset())
        .RuleFor(u => u.RoleId, _ => 1);

    public static readonly Faker<GetAccountByCredentialsDto> GetAccountByCredentialsDto =
        new Faker<GetAccountByCredentialsDto>()
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => f.Internet.Password());

    public static readonly Faker<CreateAccountDto> CreateAccountDto = new Faker<CreateAccountDto>()
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Password, f => f.Internet.Password())
        .RuleFor(u => u.PhoneNumber, f => f.Random.ReplaceNumbers("#########"))
        .RuleFor(u => u.DisplayName, f => f.Random.Word());
}
