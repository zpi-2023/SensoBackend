using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class CreateCaretakerProfileInternalDto
{
    [Required]
    public required int AccountId { get; init; }

    [Required]
    public required int SeniorId { get; init; }

    [Required]
    public required string SeniorAlias { get; init; }
}
