using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Accounts.Contracts;

public sealed record GetAccountByIdDto
{
    [Required]
    public required int Id { get; init; }
}
