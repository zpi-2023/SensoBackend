using SensoBackend.Domain.Enums;
using SensoBackend.Domain.Exceptions;

namespace SensoBackend.Application.Modules.Games.Utils;

public static class GetGame
{
    public static Game FromName(string gameName)
    {
        if (Enum.TryParse(gameName, true, out Game result))
        {
            return result;
        }
        throw new GameNotFoundException(gameName);
    }
}
