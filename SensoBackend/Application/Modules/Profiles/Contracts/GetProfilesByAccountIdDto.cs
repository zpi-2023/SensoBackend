using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public sealed record GetProfilesByAccountIdDto
{
    [Required]
    public int AccountId { get; set; }
}
