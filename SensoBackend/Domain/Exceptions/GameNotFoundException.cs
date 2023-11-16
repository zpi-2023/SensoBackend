namespace SensoBackend.Domain.Exceptions;

public class GameNotFoundException : Exception
{
    public GameNotFoundException(string gameName)
        : base($"Game {gameName} not found") { }
}
