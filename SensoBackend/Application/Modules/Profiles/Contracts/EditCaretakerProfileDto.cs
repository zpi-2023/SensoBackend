using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class EditCaretakerProfileDto
{
    [Required]
    public required string SeniorAlias { get; init; }
}
