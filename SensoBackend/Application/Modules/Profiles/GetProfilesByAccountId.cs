using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.AdditionalModels;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles.GetProfilesByAccountId;

public sealed record GetProfilesByAccountIdRequest(GetProfilesByAccountIdDto Dto)
    : IRequest<ProfilesDto>;

[UsedImplicitly]
public sealed class GetProfilesByAccountIdHandler
    : IRequestHandler<GetProfilesByAccountIdRequest, ProfilesDto>
{
    private readonly AppDbContext _context;

    public GetProfilesByAccountIdHandler(AppDbContext context) => _context = context;

    public async Task<ProfilesDto> Handle(
        GetProfilesByAccountIdRequest request,
        CancellationToken ct
    )
    {
        var profiles = await _context.Profiles
            .Where(p => p.AccountId == request.Dto.AccountId)
            .ToListAsync(ct);

        var adaptedProfiles = DtoMapper.ToProfilesDto(profiles);

        return adaptedProfiles;
    }
}
