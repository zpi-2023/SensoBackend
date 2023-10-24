using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class EncodedSeniorDto
{
    [Required]
    public required int Hash { get; init; }

    [Required]
    public required string SeniorDisplayName { get; init; }

    [Required]
    public required int ValidFor { get; init; }
}
