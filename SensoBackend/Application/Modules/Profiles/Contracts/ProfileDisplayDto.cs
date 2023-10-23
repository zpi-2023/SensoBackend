using System.ComponentModel.DataAnnotations;
using SensoBackend.Application.Modules.Profiles.Utils;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class ProfileDisplayDto
{
    [Required]
    public required string Type { get; init; }

    [Required]
    public required int SeniorId { get; init; }

    [Required]
    public required string SeniorAlias { get; init; }
}
