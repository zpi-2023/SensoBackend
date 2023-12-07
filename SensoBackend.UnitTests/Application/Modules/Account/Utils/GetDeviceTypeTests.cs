using SensoBackend.Application.Modules.Accounts.Utils;
using SensoBackend.Application.Modules.Alerts.Utils;
using SensoBackend.Domain.Enums;
using SensoBackend.Domain.Exceptions;

namespace SensoBackend.UnitTests.Application.Modules.Alerts.Utils;

public sealed class GetDeviceTypeFromNameTests
{
    [Theory]
    [InlineData("Android", DeviceType.Android)]
    [InlineData("Ios", DeviceType.Ios)]
    public void GetDeviceTypeFromName_ShouldReturnDeviceType_WhenDeviceTypeNameIsValid(
        string deviceTypeName,
        DeviceType expectedDeviceType
    )
    {
        var deviceType = GetDeviceType.FromName(deviceTypeName);

        deviceType.Should().Be(expectedDeviceType);
    }

    [Fact]
    public void GetDeviceTypeFromName_ShouldThrowIncorrectDeviceTypeNameException_WhenDeviceTypeNameIsInvalid()
    {
        var deviceTypeName = "invalidDeviceTypeName";

        var action = () => GetDeviceType.FromName(deviceTypeName);

        action
            .Should()
            .Throw<IncorrectDeviceTypeNameException>()
            .WithMessage($"Device type {deviceTypeName} is incorrect");
    }
}
