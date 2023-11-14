using SensoBackend.Application.Modules.Games;
using SensoBackend.Application.Modules.Games.Contracts;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Games;

public sealed class ReadUserBestScoreHandlerTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly ReadUserBestScoreHandler _sut;

    private readonly string _validGameName = "wordle";

    public ReadUserBestScoreHandlerTests() => _sut = new ReadUserBestScoreHandler(_context);

    [Fact]
    public async Task Handle_ShouldReturnUserBestScore()
    {
        // TODO
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
