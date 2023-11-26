using SensoBackend.Application.Modules.Medications;
using SensoBackend.Application.Modules.Medications.Contracts;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class UpdateReminderValidatorTests
{
    private readonly UpdateReminderValidator _validator = new UpdateReminderValidator();

    [Fact]
    public void Validate_ShouldReturnTrue_WhenEverythingIsOk()
    {
        var model = new UpdateReminderByIdRequest
        {
            ReminderId = 0,
            AccountId = 0,
            Dto = GetDto()
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnFalse_WhenCronIsInvalid()
    {
        var model = new UpdateReminderByIdRequest
        {
            ReminderId = 0,
            AccountId = 0,
            Dto = GetDto(cron: "Invalid")
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnTrue_WhenCronIsEveryOtherMonth()
    {
        var model = new UpdateReminderByIdRequest
        {
            ReminderId = 0,
            AccountId = 0,
            Dto = GetDto(cron: "0 0 1 */2 *")
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnTrue_WhenCronIsHourRange()
    {
        var model = new UpdateReminderByIdRequest
        {
            ReminderId = 0,
            AccountId = 0,
            Dto = GetDto(cron: "0 9-17 * * *")
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnTrue_WhenCronIsTwoDifferentHours()
    {
        var model = new UpdateReminderByIdRequest
        {
            ReminderId = 0,
            AccountId = 0,
            Dto = GetDto(cron: "0 9,17 * * *")
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    private UpdateReminderDto GetDto(
        int amountPerIntake = 1,
        int amountOwned = 1,
        string cron = "1 1 1 * * *",
        string description = "test"
    ) =>
        new UpdateReminderDto
        {
            AmountPerIntake = amountPerIntake,
            AmountOwned = amountOwned,
            Cron = cron,
            Description = description
        };
}
