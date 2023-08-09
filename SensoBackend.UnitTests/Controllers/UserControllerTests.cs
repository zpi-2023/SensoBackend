using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SensoBackend.Contracts.User;
using SensoBackend.Controllers;
using SensoBackend.Services;

namespace SensoBackend.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<ILogger<UserController>> _loggerMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();

    private UserController CreateSut()
    {
        return new UserController(_loggerMock.Object, _userServiceMock.Object);
    }

    private static IList<UserDto> GetUsersDtosList()
    {
        return new List<UserDto>
        {
            new() { Id = 0, Name = "Bernadetta Maleszka" },
            new() { Id = 1, Name = "Mariusz Fraś" }
        };
    }

    [Fact]
    private void GetAll_ShouldReturnOkWithAllUsers()
    {
        _userServiceMock.Setup(s => s.GetAll()).Returns(GetUsersDtosList());

        var sut = CreateSut();
        var actionResult = sut.GetAll();

        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();
        var result = actionResult as OkObjectResult;

        result!.Value.Should().BeOfType<List<UserDto>>();
        var returnUsers = result.Value as List<UserDto>;

        returnUsers.Should().NotBeNull();
        returnUsers.Should().HaveCount(GetUsersDtosList().Count);
    }

    [Fact]
    private void Create_ShouldReturnNoContentWhenSuccessful()
    {
        _userServiceMock.Setup(s => s.Create(It.IsAny<CreateUserDto>())).Returns(true);

        var sut = CreateSut();
        var newUser = new CreateUserDto { Name = "Bernadetta Maleszka" };
        var actionResult = sut.Create(newUser);

        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    private void Create_ShouldReturnBadRequestWhenFailed()
    {
        _userServiceMock.Setup(s => s.Create(It.IsAny<CreateUserDto>())).Returns(false);

        var sut = CreateSut();
        var newUser = new CreateUserDto { Name = "Bernadetta Maleszka" };
        var actionResult = sut.Create(newUser);

        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<BadRequestResult>();
    }
}
