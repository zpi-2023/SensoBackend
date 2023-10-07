namespace SensoBackend.Application.Modules.Users.Contracts;

public sealed record CreateUserDto
{
    public required string Name { get; init; }
}
