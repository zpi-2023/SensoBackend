using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.AdditionalModels;

public class CreateCaretakerProfileInternalDto
{
    [Required]
    public required int AccountId { get; init; }

    [Required]
    public required int EncodedSeniorId { get; init; }

    [Required]
    public required string SeniorAlias { get; init; }
}
