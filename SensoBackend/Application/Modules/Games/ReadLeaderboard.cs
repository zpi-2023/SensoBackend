using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Games.Contracts;
using SensoBackend.Application.Modules.Games.Utils;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Games;

public sealed record ReadLeaderboardRequest : IRequest<PaginatedDto<LeaderboardEntryDto>>
{
    public required string GameName { get; init; }
    public required PaginationQuery Pagination { get; init; }
}

[UsedImplicitly]
public sealed class ReadLeaderboardHandler(AppDbContext context)
    : IRequestHandler<ReadLeaderboardRequest, PaginatedDto<LeaderboardEntryDto>>
{
    public async Task<PaginatedDto<LeaderboardEntryDto>> Handle(
        ReadLeaderboardRequest request,
        CancellationToken ct
    )
    {
        var game = GetGame.FromName(request.GameName);

        var leaderboard = await context
            .LeaderboardEntries
            .Where(l => l.Game == game)
            .OrderByDescending(l => l.Score)
            .Paged(request.Pagination)
            .ToListAsync(ct);

        return new PaginatedDto<LeaderboardEntryDto>
        {
            Items = leaderboard.Adapt<List<LeaderboardEntryDto>>(),
        };
    }
}
