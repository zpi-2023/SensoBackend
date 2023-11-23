using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles;

public sealed record GetProfilesByAccountIdRequest : IRequest<ProfilesDto>
{
    public required int AccountId { get; init; }
}

[UsedImplicitly]
public sealed class GetProfilesByAccountIdHandler(AppDbContext context)
    : IRequestHandler<GetProfilesByAccountIdRequest, ProfilesDto>
{
    public async Task<ProfilesDto> Handle(
        GetProfilesByAccountIdRequest request,
        CancellationToken ct
    )
    {
        var profiles = await context
            .Profiles
            .Where(p => p.AccountId == request.AccountId)
            .ToListAsync(ct);

        return profiles.Adapt<ProfilesDto>();
    }
}
