using FluentValidation;
using SensoBackend.Application.Modules.Games;
using SensoBackend.Application.Modules.Games.Contracts;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Games;

public sealed class TryUpdateUserBestScoreHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly TryUpdateUserBestScoreHandler _sut;

    private readonly string _validGameName = "wordle";

    public TryUpdateUserBestScoreHandlerTests() =>
        _sut = new TryUpdateUserBestScoreHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldUpdateUserBestScore()
    {
        // TODO
    }

    [Fact]
    public async Task Handle_ShouldCreateUserBestScore_WhenScoreDoesNotExists()
    {
        // TODO
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateUserBestScore_WhenScoreIsLowerThanCurrentBestScore()
    {
        // TODO
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateUserBestScore_WhenScoreIsEqualToCurrentBestScore()
    {
        // TODO
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
