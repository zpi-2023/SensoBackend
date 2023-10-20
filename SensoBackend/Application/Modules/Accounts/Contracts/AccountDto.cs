using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Accounts.Contracts;

public sealed record AccountDto
{
    [Required]
    public required int Id { get; init; }

    [Required]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }

    [Required]
    public required bool Active { get; init; }

    [Required]
    public required bool Verified { get; init; }
    public string? PhoneNumber { get; init; }
    public string? DisplayName { get; init; }

    [Required]
    public required DateTimeOffset CreatedAt { get; init; }

    [Required]
    public required DateTimeOffset LastLoginAt { get; init; }

    [Required]
    public required DateTimeOffset LastPasswordChangeAt { get; init; }
    
    [Required]
    public required int RoleId { get; init; }
}
