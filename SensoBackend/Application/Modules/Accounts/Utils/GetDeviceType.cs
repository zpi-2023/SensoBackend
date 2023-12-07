using SensoBackend.Domain.Enums;
using SensoBackend.Domain.Exceptions;

namespace SensoBackend.Application.Modules.Accounts.Utils;

public static class GetDeviceType
{
    public static DeviceType FromName(string deviceTypeName)
    {
        if (Enum.TryParse(deviceTypeName, true, out DeviceType result))
        {
            return result;
        }
        throw new IncorrectDeviceTypeNameException(deviceTypeName);
    }
}
