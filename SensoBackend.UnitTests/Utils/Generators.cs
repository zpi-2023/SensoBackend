using Bogus;
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
}
