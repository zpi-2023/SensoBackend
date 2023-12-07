using SensoBackend.Domain.Enums;
using SensoBackend.Domain.Exceptions;

namespace SensoBackend.Application.Modules.Alerts.Utils;

public static class GetAlertType
{
    public static AlertType FromName(string alertTypeName)
    {
        if (Enum.TryParse(alertTypeName, true, out AlertType result))
        {
            return result;
        }
        throw new AlertTypeNotFoundException(alertTypeName);
    }
}
