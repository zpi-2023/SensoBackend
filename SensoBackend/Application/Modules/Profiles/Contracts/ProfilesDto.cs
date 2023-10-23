using System.ComponentModel.DataAnnotations;
using SensoBackend.Application.Modules.Profiles.Utils;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class ProfilesDto
{
    [Required]
    public required List<ProfileDisplayDto> Profiles { get; init; }
}
