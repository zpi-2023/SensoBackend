using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Profiles.Contracts;

public class ProfilesDto
{
    [Required]
    public List<ProfileDisplayDto> Profiles { get; init; }
    
    public ProfilesDto(List<ProfileDto> profiles)
    {
        var properProfiles = profiles.Select(p => new ProfileDisplayDto(p)).ToList();
        Profiles = properProfiles;
    }
}
