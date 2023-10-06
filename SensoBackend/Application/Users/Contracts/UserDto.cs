namespace SensoBackend.Application.Users.Contracts;

public sealed record UserDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}
