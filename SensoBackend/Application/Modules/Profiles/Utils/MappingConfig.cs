using Mapster;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Application.Modules.Profiles.Utils;

public sealed class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .ForType<Profile, ProfileDisplayDto>()
            .Map(dst => dst.Type, src => src.SeniorId == src.AccountId ? "senior" : "caretaker")
            .Map(dst => dst.SeniorAlias, src => src.Alias);

        config
            .ForType<List<Profile>, ProfilesDto>()
            .Map(
                dst => dst.Profiles,
                src => src.Select(p => p.Adapt<ProfileDisplayDto>()).ToList()
            );

        config
            .ForType<List<ExtendedProfileDto>, ExtendedProfilesDto>()
            .Map(
                dst => dst.Profiles,
                src => src.Select(p => p.Adapt<ExtendedProfileDto>()).ToList()
            );
    }
}
