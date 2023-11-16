using Mapster;
using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Games;
using SensoBackend.Application.Modules.Games.Contracts;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Games;

public sealed class ReadLeaderboardHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly ReadLeaderboardHandler _sut;

    private static readonly string _validGameName = "wordle";
    private static readonly Game _game = Game.Wordle;
    private static readonly PaginationQuery _defaultPaginationQuery =
        new() { Offset = 0, Limit = 10 };

    public ReadLeaderboardHandlerTests() => _sut = new ReadLeaderboardHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnLeaderboard()
    {
        var expectedLeaderboard = new List<LeaderboardEntryDto>();
        for (int i = 0; i < _defaultPaginationQuery.Limit; i++)
        {
            var leaderboardEntry = await _context.SetupLeaderboardEntry(_game, i);
            expectedLeaderboard.Add(leaderboardEntry.Adapt<LeaderboardEntryDto>());
        }

        var leaderboard = await _sut.Handle(
            new ReadLeaderboardRequest
            {
                GameName = _validGameName,
                Pagination = _defaultPaginationQuery
            },
            CancellationToken.None
        );

        leaderboard.Should().BeOfType<PaginatedDto<LeaderboardEntryDto>>();
        leaderboard.Items.Should().BeEquivalentTo(expectedLeaderboard);
        leaderboard.Items.Should().BeInDescendingOrder(l => l.Score);
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

        leaderboard.Should().BeOfType<PaginatedDto<LeaderboardEntryDto>>();
        leaderboard.Items.Should().BeEmpty();
    }
}
