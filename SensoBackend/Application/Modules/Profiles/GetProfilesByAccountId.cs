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
        var profiles = await _context
            .Profiles
            .Where(p => p.AccountId == request.AccountId)
            .ToListAsync(ct);

        return profiles.Adapt<ProfilesDto>();
    }
}
