using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.AdditionalModels;

public sealed record GetProfilesByAccountIdDto
{
    [Required]
    public int AccountId { get; init; }
}
