using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Games.Contracts;
using SensoBackend.Application.Modules.Games.Utils;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Games;

public sealed record TryUpdateUserBestScoreRequest : IRequest
{
    public required int AccountId { get; init; }
    public required string GameName { get; init; }
    public required ScoreDto Dto { get; init; }
}

[UsedImplicitly]
public sealed class TryUpdateUserBestScoreValidator
    : AbstractValidator<TryUpdateUserBestScoreRequest>
{
    public TryUpdateUserBestScoreValidator()
    {
        RuleFor(r => r.Dto.Score).GreaterThanOrEqualTo(0);
    }
}

[UsedImplicitly]
public sealed class TryUpdateUserBestScoreHandler : IRequestHandler<TryUpdateUserBestScoreRequest>
{
    private readonly AppDbContext _context;

    public TryUpdateUserBestScoreHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(TryUpdateUserBestScoreRequest request, CancellationToken ct)
    {
        var game = GetGame.FromName(request.GameName);

        var score = await _context.LeaderboardEntries.FirstOrDefaultAsync(
            s => s.AccountId == request.AccountId && s.Game == game,
            ct
        );

        if (score is null)
        {
            await CreateLeaderboardEntry(request.AccountId, game, request.Dto.Score, ct);
        }
        else if (score.Score < request.Dto.Score)
        {
            score.Score = request.Dto.Score;
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task CreateLeaderboardEntry(
        int accountId,
        Game game,
        int score,
        CancellationToken ct
    )
    {
        var leaderboardEntry = new LeaderboardEntry
        {
            Id = default,
            AccountId = accountId,
            Game = game,
            Score = score,
        };

        await _context.LeaderboardEntries.AddAsync(leaderboardEntry, ct);
    }
}
