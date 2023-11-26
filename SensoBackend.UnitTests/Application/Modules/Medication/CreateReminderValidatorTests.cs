using SensoBackend.Application.Modules.Medications;
using SensoBackend.Application.Modules.Medications.Contracts;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class CreateReminderValidatorTests
{
    private static readonly DateTimeOffset Now = new(2015, 4, 12, 18, 38, 5, TimeSpan.Zero);
    private readonly CreateReminderValidator _validator = new CreateReminderValidator();

    [Fact]
    public void Validate_ShouldReturnTrue_WhenEverythingIsOk()
    {
        var model = new CreateReminderRequest
        {
            AccountId = 0,
            SeniorId = 0,
            Dto = GetDto()
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnFalse_WhenMedicationNameIsEmpty()
    {
        var model = new CreateReminderRequest
        {
            AccountId = 0,
            SeniorId = 0,
            Dto = GetDto(medicationName: "")
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnFalse_WhenMedicationAmountInPackageIs0()
    {
        var model = new CreateReminderRequest
        {
            AccountId = 0,
            SeniorId = 0,
            Dto = GetDto(medicationAmountInPackage: 0)
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnFalse_WhenAmountOwnedIsLessThan0()
    {
        var model = new CreateReminderRequest
        {
            AccountId = 0,
            SeniorId = 0,
            Dto = GetDto(amountOwned: -1)
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnFalse_WhenAmountPerIntakeIs0()
    {
        var model = new CreateReminderRequest
        {
            AccountId = 0,
            SeniorId = 0,
            Dto = GetDto(amountPerIntake: 0)
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnFalse_WhenCronIsInvalid()
    {
        var model = new CreateReminderRequest
        {
            AccountId = 0,
            SeniorId = 0,
            Dto = GetDto(cron: "Invalid")
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnTrue_WhenCronIsEveryOtherMonth()
    {
        var model = new CreateReminderRequest
        {
            AccountId = 0,
            SeniorId = 0,
            Dto = GetDto(cron: "0 0 1 */2 *")
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnTrue_WhenCronIsHourRange()
    {
        var model = new CreateReminderRequest
        {
            AccountId = 0,
            SeniorId = 0,
            Dto = GetDto(cron: "0 9-17 * * *")
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnTrue_WhenCronIsTwoDifferentHours()
    {
        var model = new CreateReminderRequest
        {
            AccountId = 0,
            SeniorId = 0,
            Dto = GetDto(cron: "0 9,17 * * *")
        };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    private CreateReminderDto GetDto(
        string medicationName = "test",
        int medicationAmountInPackage = 1,
        int amountPerIntake = 1,
        int amountOwned = 1,
        string amountUnit = "g",
        string cron = "1 1 1 * * *",
        string description = "test"
    ) =>
        new CreateReminderDto
        {
            MedicationName = medicationName,
            MedicationAmountInPackage = medicationAmountInPackage,
            AmountPerIntake = amountPerIntake,
            AmountOwned = amountOwned,
            AmountUnit = amountUnit,
            Cron = cron,
            Description = description,
        };
}
