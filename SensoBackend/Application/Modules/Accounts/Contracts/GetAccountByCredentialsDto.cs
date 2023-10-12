using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Accounts.Contracts;

public sealed record GetAccountByCredentialsDto
{
    [Required]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }
}
