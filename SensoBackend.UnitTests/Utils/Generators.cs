using Bogus;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Users.Contracts;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Tests.Utils;

public static class Generators
{
    static Generators()
    {
        Faker.DefaultStrictMode = true;
        Randomizer.Seed = new Random(0);
    }

    public static readonly Faker<CreateUserDto> CreateUserDto = new Faker<CreateUserDto>().RuleFor(
        u => u.Name,
        f => f.Name.FullName()
    );

    public static readonly Faker<UserDto> UserDto = new Faker<UserDto>()
        .RuleFor(u => u.Id, f => f.IndexVariable++)
        .RuleFor(u => u.Name, f => f.Name.FullName());

    public static readonly Faker<User> User = new Faker<User>()
        .RuleFor(u => u.Id, _ => 0)
        .RuleFor(u => u.Name, f => f.Name.FullName());

    public static readonly Faker<AccountDto> AccountDto = new Faker<AccountDto>()
        .RuleFor(u => u.Id, f => f.IndexVariable++)
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Password, f => f.Internet.Password())
        .RuleFor(u => u.Active, _ => true)
        .RuleFor(u => u.Verified, _ => true)
        .RuleFor(u => u.PhoneNumber, _ => "123456789")
        .RuleFor(u => u.DisplayName, f => f.Name.FullName())
        .RuleFor(u => u.CreatedAt, f => f.Date.PastOffset())
        .RuleFor(u => u.LastLoginAt, f => f.Date.PastOffset())
        .RuleFor(u => u.LastPasswordChangeAt, f => f.Date.PastOffset());

    public static readonly Faker<GetAccountByCredentialsDto> GetAccountByCredentialsDto =
        new Faker<GetAccountByCredentialsDto>()
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => f.Internet.Password());

    public static readonly Faker<CreateAccountDto> CreateAccountDto = new Faker<CreateAccountDto>()
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Password, f => f.Internet.Password())
        .RuleFor(u => u.PhoneNumber, _ => "123456789");
}
