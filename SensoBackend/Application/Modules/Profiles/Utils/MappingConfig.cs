using Mapster;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Application.Modules.Profiles.Utils;

public sealed class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .ForType<List<Profile>, ProfilesDto>()
            .Map(
                dest => dest.Profiles,
                src =>
                    src.Select(
                            p =>
                                new ProfileDisplayDto
                                {
                                    Type = p.SeniorId == p.AccountId ? "senior" : "caretaker",
                                    SeniorId = p.SeniorId,
                                    SeniorAlias = p.Alias
                                }
                        )
                        .ToList()
            );
    }
}
