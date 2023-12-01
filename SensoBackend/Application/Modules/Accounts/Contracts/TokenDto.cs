using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Accounts.Contracts;

public sealed record TokenDto
{
    [Required]
    public required string Token { get; init; }

    [Required]
    public required int AccountId { get; init; }
}
