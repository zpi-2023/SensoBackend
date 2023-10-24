using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Application.Modules.Profiles.Utils;

public static class DtoMapper
{
    public static ProfilesDto ToProfilesDto(List<Profile> profiles)
    {
        List<ProfileDisplayDto> displayProfiles = new List<ProfileDisplayDto>();
        foreach (var profile in profiles)
        {
            displayProfiles.Add(new ProfileDisplayDto
            { 
                Type = profile.SeniorId == profile.AccountId
                    ? "senior"
                    : "caretaker",
                SeniorId = profile.SeniorId,
                SeniorAlias = profile.Alias
            });
        }

        return new ProfilesDto { Profiles = displayProfiles };
    }
}
