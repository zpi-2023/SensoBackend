namespace SensoBackend.Application.Modules.Accounts.Contracts;

public sealed record ValidateAccountDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
