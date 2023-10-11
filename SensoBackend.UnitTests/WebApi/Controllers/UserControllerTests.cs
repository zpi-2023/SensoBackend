using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SensoBackend.Application.Modules.Users.Contracts;
using SensoBackend.Application.Modules.Users.GetUsers;
using SensoBackend.Tests.Utils;
using SensoBackend.WebApi.Controllers.V1;

namespace SensoBackend.Tests.WebApi.Controllers;

public sealed class UserControllerTests
{
    private readonly ILogger<UserController> _logger = Substitute.For<ILogger<UserController>>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly UserController _sut;

    public UserControllerTests() => _sut = new UserController(_logger, _mediator);

    [Fact]
    public void GetAll_ShouldReturnOkWithAllUsers()
    {
        const int count = 3;
        _mediator.Send(Arg.Any<GetUsersRequest>()).Returns(Generators.UserDto.Generate(count));

        var actionResult = _sut.GetAll();

        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        var result = actionResult.Result as OkObjectResult;

        result!.Value.Should().BeOfType<List<UserDto>>();
        var returnUsers = result.Value as List<UserDto>;

        returnUsers.Should().NotBeNull();
        returnUsers.Should().HaveCount(count);
    }

    [Fact]
    public void Create_ShouldReturnNoContent_WhenSuccessful()
    {
        var newUser = Generators.CreateUserDto.Generate();
        var actionResult = _sut.Create(newUser);

        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<NoContentResult>();
    }
}
