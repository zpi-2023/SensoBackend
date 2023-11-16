using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Games;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Games;

public sealed class ReadLeaderboardHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly ReadLeaderboardHandler _sut;

    private readonly string _validGameName = "wordle";
    private readonly PaginationQuery _defaultPaginationQuery = new() { Offset = 0, Limit = 10 };

    public ReadLeaderboardHandlerTests() => _sut = new ReadLeaderboardHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnLeaderboard()
    {
        // TODO
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyLeaderboard_WhenLeaderboardIsEmpty()
    {
        var leaderboard = await _sut.Handle(
            new ReadLeaderboardRequest
            {
                GameName = _validGameName,
                Pagination = _defaultPaginationQuery
            },
            CancellationToken.None
        );

        leaderboard.Items.Should().BeEmpty();
    }
}
