using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles;

public sealed record GetListOfProfilesByAccountIdRequest(int AccountId)
    : IRequest<List<ProfileInfo>>;

[UsedImplicitly]
public sealed class GetListOfProfilesByAccountIdHandler
    : IRequestHandler<GetListOfProfilesByAccountIdRequest, List<ProfileInfo>>
{
    private readonly AppDbContext _context;

    public GetListOfProfilesByAccountIdHandler(AppDbContext context) => _context = context;

    public async Task<List<ProfileInfo>> Handle(
        GetListOfProfilesByAccountIdRequest request,
        CancellationToken ct
    )
    {
        var profiles = await _context.Profiles
            .Where(p => p.AccountId == request.AccountId)
            .ToListAsync(ct);

        return profiles.Adapt<List<ProfileInfo>>();
    }
}
