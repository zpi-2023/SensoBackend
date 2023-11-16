using SensoBackend.Application.Modules.Games.Utils;
using SensoBackend.Domain.Enums;
using SensoBackend.Domain.Exceptions;

namespace SensoBackend.UnitTests.Application.Modules.Games.Utils;

public sealed class GetGameFromNameTests
{
    [Theory]
    [InlineData("Wordle", Game.Wordle)]
    [InlineData("Sudoku", Game.Sudoku)]
    [InlineData("Memory", Game.Memory)]
    public void GetGameFromName_ShouldReturnGame_WhenGameNameIsValid(
        string validGameName,
        Game expectedGame
    )
    {
        var game = GetGame.FromName(validGameName);

        game.Should().Be(expectedGame);
    }

    [Fact]
    public void GetGameFromName_ShouldThrowGameNotFoundException_WhenGameNameIsInvalid()
    {
        var invaliGameName = "invalidGameName";

        var action = () => GetGame.FromName(invaliGameName);

        action
            .Should()
            .Throw<GameNotFoundException>()
            .WithMessage($"Game {invaliGameName} not found");
    }
}
