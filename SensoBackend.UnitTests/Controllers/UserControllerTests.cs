using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SensoBackend.Contracts.User;
using SensoBackend.Controllers;
using SensoBackend.Services;

namespace SensoBackend.Tests.Controllers;

public class UserControllerTests
{
    // Ważna kwestia do przemyślenia:
    // Czy mockujemy logger
    // Czy wstawiamy legitny logger, który pozwoli na analizę logów z testów
    // To oczywiście zakłada, że będziemy tworzyć sensowne logi :)
    private readonly Mock<ILogger<UserController>> _loggerMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();

    private UserController CreateSut()
    {
        return new UserController(_loggerMock.Object, _userServiceMock.Object);
    }

    private static IEnumerable<UserDto> GetUsersDtosList()
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

        Assert.NotNull(actionResult.Result);
        Assert.Equal(typeof(OkObjectResult), actionResult.Result.GetType());

        var result = actionResult.Result as OkObjectResult;
        var returnUsers = result!.Value as IEnumerable<UserDto>;

        Assert.NotNull(returnUsers);
        Assert.Equal(GetUsersDtosList().Count(), returnUsers.Count());
    }

    [Fact]
    private void Create_ShouldReturnNoContentWhenSuccessful()
    {
        _userServiceMock.Setup(s => s.Create(It.IsAny<CreateUserDto>())).Returns(true);

        var sut = CreateSut();
        var newUser = new CreateUserDto { Name = "Bernadetta Maleszka" };
        var actionResult = sut.Create(newUser);

        Assert.NotNull(actionResult.Result);
        Assert.Equal(typeof(NoContentResult), actionResult.Result.GetType());
    }

    [Fact]
    private void Create_ShouldReturnBadRequestWhenFailed()
    {
        _userServiceMock.Setup(s => s.Create(It.IsAny<CreateUserDto>())).Returns(false);

        var sut = CreateSut();
        var newUser = new CreateUserDto { Name = "Bernadetta Maleszka" };
        var actionResult = sut.Create(newUser);

        Assert.NotNull(actionResult.Result);
        Assert.Equal(typeof(BadRequestResult), actionResult.Result.GetType());
    }
}
