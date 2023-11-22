using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class ExtendedProfileDto
{
    [Required]
    public required int AccountId { get; init; }

    [Required]
    public required int SeniorId { get; init; }

    [Required]
    public required string Type { get; init; }

    [Required]
    public required string DisplayName { get; init; }

    [Required]
    public required string Email { get; init; }

    [Required]
    public required string PhoneNumber { get; init; }
}

public class ExtendedProfilesDto
{
    [Required]
    public required List<ExtendedProfileDto> Profiles { get; init; }
}
