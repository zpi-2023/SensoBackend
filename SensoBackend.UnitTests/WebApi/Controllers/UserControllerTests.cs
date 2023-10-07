using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SensoBackend.Application.Modules.Users.Contracts;
using SensoBackend.Application.Modules.Users.CreateUser;
using SensoBackend.Application.Modules.Users.GetUsers;
using SensoBackend.WebApi.Controllers;

namespace SensoBackend.Tests.WebApi.Controllers;

public sealed class UserControllerTests
{
    private readonly Mock<ILogger<UserController>> _loggerMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly UserController _sut;

    public UserControllerTests() =>
        _sut = new UserController(_loggerMock.Object, _mediatorMock.Object);

    private static IList<UserDto> GetUsersDtoList() =>
        new List<UserDto>
        {
            new() { Id = 0, Name = "Bernadetta Maleszka" },
            new() { Id = 1, Name = "Mariusz Fraś" }
        };

    [Fact]
    public void GetAll_ShouldReturnOkWithAllUsers()
    {
        _mediatorMock
            .Setup(s => s.Send(It.IsAny<GetUsersRequest>(), default))
            .ReturnsAsync(GetUsersDtoList());

        var actionResult = _sut.GetAll();

        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        var result = actionResult.Result as OkObjectResult;

        result!.Value.Should().BeOfType<List<UserDto>>();
        var returnUsers = result.Value as List<UserDto>;

        returnUsers.Should().NotBeNull();
        returnUsers.Should().HaveCount(GetUsersDtoList().Count);
    }

    [Fact]
    public void Create_ShouldReturnNoContent_WhenSuccessful()
    {
        _mediatorMock
            .Setup(s => s.Send(It.IsAny<CreateUserRequest>(), default))
            .Returns(Task.CompletedTask);

        var newUser = new CreateUserDto { Name = "Bernadetta Maleszka" };
        var actionResult = _sut.Create(newUser);

        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<NoContentResult>();
    }
}
