using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class CreateCaretakerProfileDto
{
    [Required]
    public required string EncodedSeniorId { get; init; }

    [Required]
    public required string SeniorAlias { get; init; }
}
