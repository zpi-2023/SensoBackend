using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Accounts.Contracts;

public sealed record AccountDto
{
    [Required]
    public required int Id { get; init; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required bool Active { get; set; }

    [Required]
    public required bool Verified { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DisplayName { get; set; }

    [Required]
    public required DateTimeOffset CreatedAt { get; set; }

    [Required]
    public required DateTimeOffset LastLoginAt { get; set; }

    [Required]
    public required DateTimeOffset LastPasswordChangeAt { get; set; }
    
    [Required]
    public required int RoleId { get; set; }
}
