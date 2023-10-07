using Microsoft.AspNetCore.Mvc;
using Mapster;
using MediatR;
using SensoBackend.Application.Modules.Users.Contracts;
using SensoBackend.Application.Modules.Users.CreateUser;
using SensoBackend.Application.Modules.Users.GetUsers;

namespace SensoBackend.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediator;

    public UserController(ILogger<UserController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET: GetAll");
        var response = await _mediator.Send(new GetUsersRequest());
        var users = response.Adapt<IList<UserDto>>();
        return Ok(users);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        _logger.LogInformation("POST: Create");
        await _mediator.Send(dto.Adapt<CreateUserRequest>());
        return NoContent();
    }
}
