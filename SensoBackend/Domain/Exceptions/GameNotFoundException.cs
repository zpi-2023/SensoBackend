namespace SensoBackend.Domain.Exceptions;

public class GameNotFoundException(string gameName) : Exception($"Game {gameName} not found") { }
