using SensoBackend.Application.Modules.Games;
using SensoBackend.Application.Modules.Games.Contracts;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Games;

public sealed class ReadUserBestScoreHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly ReadUserBestScoreHandler _sut;

    private static readonly string _validGameName = "wordle";
    private static readonly Game _game = Game.Wordle;

    public ReadUserBestScoreHandlerTests() => _sut = new ReadUserBestScoreHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnUserBestScore()
    {
        var account = await _context.SetUpAccount();
        var leaderboardEntry = await _context.SetupLeaderboardEntry(_game, account, 1);

        var userBestScore = await _sut.Handle(
            new ReadUserBestScoreRequest { AccountId = account.Id, GameName = _validGameName },
            CancellationToken.None
        );

        userBestScore.Should().BeOfType<ScoreDto>();
        userBestScore.Score.Should().Be(leaderboardEntry.Score);
    }

    [Fact]
    public async Task Handle_ShouldReturnZero_WhenUserBestScoreDoesNotExist()
    {
        var userBestScore = await _sut.Handle(
            new ReadUserBestScoreRequest { AccountId = 0, GameName = _validGameName },
            CancellationToken.None
        );

        userBestScore.Should().BeOfType<ScoreDto>();
        userBestScore.Score.Should().Be(0);
    }
}
