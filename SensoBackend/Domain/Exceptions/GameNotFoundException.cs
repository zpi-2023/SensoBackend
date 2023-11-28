using SensoBackend.Domain.Exceptions.Abstractions;

namespace SensoBackend.Domain.Exceptions;

public class GameNotFoundException(string gameName)
    : ApiException(404, $"Game {gameName} not found") { }
