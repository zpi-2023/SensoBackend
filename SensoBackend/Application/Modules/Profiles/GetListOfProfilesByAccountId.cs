﻿using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles;

public sealed record GetListOfProfilesByAccountIdRequest : IRequest<List<ProfileInfo>>
{
    public required int AccountId { get; init; }
}

[UsedImplicitly]
public sealed class GetListOfProfilesByAccountIdHandler(AppDbContext context)
    : IRequestHandler<GetListOfProfilesByAccountIdRequest, List<ProfileInfo>>
{
    public async Task<List<ProfileInfo>> Handle(
        GetListOfProfilesByAccountIdRequest request,
        CancellationToken ct
    )
    {
        var profiles = await context
            .Profiles
            .Where(p => p.AccountId == request.AccountId)
            .ToListAsync(ct);

        return profiles.Adapt<List<ProfileInfo>>();
    }
}
