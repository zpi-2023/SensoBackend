using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Accounts.Contracts;

public sealed record GetAccountDto
{
    [Required]
    public required int Id { get; init; }

    [Required]
    public required string Email { get; init; }

    [Required]
    public required string DisplayName { get; init; }

    public string? PhoneNumber { get; init; }
}
