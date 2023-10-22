using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public sealed record CreateSeniorProfileDto
{
    [Required]
    public int AccountId { get; init; }
}
