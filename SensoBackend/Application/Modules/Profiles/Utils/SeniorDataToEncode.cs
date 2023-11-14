using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Utils;

public class SeniorDataToEncode
{
    [Required]
    public required int SeniorId { get; init; }

    [Required]
    public required string SeniorDisplayName { get; init; }

    [Required]
    public required DateTimeOffset ValidTo { get; init; }
}
