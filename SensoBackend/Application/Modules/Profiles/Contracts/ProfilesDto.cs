using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class ProfilesDto
{
    [Required]
    public required List<ProfileDisplayDto> Profiles { get; init; }
}
