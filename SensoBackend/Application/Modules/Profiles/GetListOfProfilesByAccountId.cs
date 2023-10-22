using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles.GetListOfProfilesByAccountId;

public sealed record GetListOfProfilesByAccountIdRequest(GetProfilesByAccountIdDto Dto)
    : IRequest<List<ProfileDto>>;

[UsedImplicitly]
public sealed class GetListOfProfilesByAccountIdHandler
    : IRequestHandler<GetListOfProfilesByAccountIdRequest, List<ProfileDto>>
{
    private readonly AppDbContext _context;

    public GetListOfProfilesByAccountIdHandler(AppDbContext context) => _context = context;

    public async Task<List<ProfileDto>> Handle(
        GetListOfProfilesByAccountIdRequest request,
        CancellationToken ct
    )
    {
        var profiles = await _context.Profiles
            .Where(p => p.AccountId == request.Dto.AccountId)
            .ToListAsync(ct);

        return profiles.Adapt<List<ProfileDto>>();
    }
}
