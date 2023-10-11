namespace SensoBackend.Application.Modules.Accounts.Contracts;

public sealed record CreateAccountDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? PhoneNumber { get; init; }
}
