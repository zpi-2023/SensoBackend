using SensoBackend.Application.Modules.Users.Contracts;
using SensoBackend.Application.Modules.Users.CreateUser;

namespace SensoBackend.Tests.Application.Modules.Users.CreateUser;

public sealed class CreateUserValidatorTests
{
    private readonly CreateUserValidator _sut = new();

    [Fact]
    public void Validate_ShouldReturnValid_WhenRequestIsValid()
    {
        var request = new CreateUserRequest(new CreateUserDto { Name = "Mariusz Fraś" });

        var result = _sut.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnInvalid_WhenUserNameIsEmpty()
    {
        var request = new CreateUserRequest(new CreateUserDto { Name = "" });

        var result = _sut.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnInvalid_WhenUserNameIsTooLong()
    {
        var request = new CreateUserRequest(
            new CreateUserDto { Name = string.Concat(Enumerable.Repeat('a', 100)) }
        );

        var result = _sut.Validate(request);

        result.IsValid.Should().BeFalse();
    }
}
