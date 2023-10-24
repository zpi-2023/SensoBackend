using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Accounts.Contracts;

public sealed record CreateAccountDto
{
    [Required]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }

    public string? PhoneNumber { get; init; }

    [Required]
    public required string DisplayName { get; init; }
}
