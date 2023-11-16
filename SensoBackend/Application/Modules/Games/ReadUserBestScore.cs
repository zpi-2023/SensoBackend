using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Games.Contracts;
using SensoBackend.Application.Modules.Games.Utils;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Games;

public sealed record ReadUserBestScoreRequest : IRequest<ScoreDto>
{
    public required int AccountId { get; init; }
    public required string GameName { get; init; }
}

[UsedImplicitly]
public sealed class ReadUserBestScoreHandler : IRequestHandler<ReadUserBestScoreRequest, ScoreDto>
{
    private readonly AppDbContext _context;

    public ReadUserBestScoreHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ScoreDto> Handle(ReadUserBestScoreRequest request, CancellationToken ct)
    {
        var game = GetGame.FromName(request.GameName);

        var score = await _context.LeaderboardEntries.FirstOrDefaultAsync(
            s => s.AccountId == request.AccountId && s.Game == game,
            ct
        );

        return new ScoreDto { Score = score?.Score ?? 0 };
    }
}
