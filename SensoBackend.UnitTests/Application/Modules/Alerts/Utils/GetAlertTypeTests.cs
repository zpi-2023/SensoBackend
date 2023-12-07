using SensoBackend.Application.Modules.Alerts.Utils;
using SensoBackend.Domain.Enums;
using SensoBackend.Domain.Exceptions;

namespace SensoBackend.UnitTests.Application.Modules.Alerts.Utils;

public sealed class GetAlertTypeFromNameTests
{
    [Theory]
    [InlineData("Sos", AlertType.sos)]
    [InlineData("MedicationToTake", AlertType.medicationToTake)]
    [InlineData("medicationNotTaken", AlertType.medicationNotTaken)]
    public void GetAlertTypeFromName_ShouldReturnGame_WhenAlertTypeNameIsValid(
        string alertTypeName,
        AlertType expectedAlertType
    )
    {
        var alert = GetAlertType.FromName(alertTypeName);

        alert.Should().Be(expectedAlertType);
    }

    [Fact]
    public void GetAlertTypeFromName_ShouldThrowAlertTypeNotFoundException_WhenAlertTypeNameIsInvalid()
    {
        var alertTypeName = "invalidAlertTypeName";

        var action = () => GetAlertType.FromName(alertTypeName);

        action
            .Should()
            .Throw<AlertTypeNotFoundException>()
            .WithMessage($"Alert type {alertTypeName} not found");
    }
}
