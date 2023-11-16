using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Games;
using SensoBackend.Application.Modules.Games.Contracts;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Games;

public sealed class TryUpdateUserBestScoreHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly TryUpdateUserBestScoreHandler _sut;

    private static readonly string _validGameName = "wordle";
    private static readonly Game _game = Game.Wordle;

    public TryUpdateUserBestScoreHandlerTests() =>
        _sut = new TryUpdateUserBestScoreHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldUpdateUserBestScore()
    {
        var account = await _context.SetUpAccount();
        var leaderboardEntry = await _context.SetupLeaderboardEntry(_game, account, 1);
        var newBestScoreDto = new ScoreDto { Score = leaderboardEntry.Score + 1 };

        await _sut.Handle(
            new TryUpdateUserBestScoreRequest
            {
                AccountId = account.Id,
                GameName = _validGameName,
                Dto = newBestScoreDto
            },
            CancellationToken.None
        );

        var updatedLeaderboardEntry = await _context
            .LeaderboardEntries
            .FirstAsync(l => l.AccountId == account.Id && l.Game == _game);

        updatedLeaderboardEntry.Score.Should().Be(newBestScoreDto.Score);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserBestScore_WhenScoreDoesNotExists()
    {
        var account = await _context.SetUpAccount();
        var newBestScoreDto = new ScoreDto { Score = 1 };

        await _sut.Handle(
            new TryUpdateUserBestScoreRequest
            {
                AccountId = account.Id,
                GameName = _validGameName,
                Dto = newBestScoreDto
            },
            CancellationToken.None
        );

        var updatedLeaderboardEntry = await _context
            .LeaderboardEntries
            .FirstAsync(l => l.AccountId == account.Id && l.Game == _game);

        updatedLeaderboardEntry.Score.Should().Be(newBestScoreDto.Score);
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateUserBestScore_WhenScoreIsLowerThanCurrentBestScore()
    {
        var account = await _context.SetUpAccount();
        var leaderboardEntry = await _context.SetupLeaderboardEntry(_game, account, 2);
        var newBestScoreDto = new ScoreDto { Score = leaderboardEntry.Score - 1 };

        await _sut.Handle(
            new TryUpdateUserBestScoreRequest
            {
                AccountId = account.Id,
                GameName = _validGameName,
                Dto = newBestScoreDto
            },
            CancellationToken.None
        );

        var updatedLeaderboardEntry = await _context
            .LeaderboardEntries
            .FirstAsync(l => l.AccountId == account.Id && l.Game == _game);

        updatedLeaderboardEntry.Score.Should().Be(leaderboardEntry.Score);
    }
}

public sealed class TryUpdateUserBestScoreValidatorTests
{
    private readonly TryUpdateUserBestScoreValidator _sut = new();
    private readonly string _validGameName = "wordle";

    [Fact]
    public void Validate_ShouldThrowValidationException_WhenScoreIsNegative()
    {
        var scoreDto = new ScoreDto { Score = -1 };

        var request = new TryUpdateUserBestScoreRequest
        {
            AccountId = 0,
            GameName = _validGameName,
            Dto = scoreDto
        };

        var act = () => _sut.ValidateAndThrow(request);

        act.Should().Throw<ValidationException>();
    }
}
