namespace SensoBackend.Domain.Exceptions;

public class GameNotFoundException : Exception
{
    public GameNotFoundException(string gameName)
        : base($"Could not find game with name: {gameName}") { }
}
