using Microsoft.AspNetCore.Mvc;
using Mapster;
using MediatR;
using SensoBackend.Application.Users.Contracts;
using SensoBackend.Application.Users.CreateUser;
using SensoBackend.Application.Users.GetUsers;

namespace SensoBackend.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserController> _logger;

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
        var users = await _mediator.Send(new GetUsersRequest());
        return Ok(users);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        _logger.LogInformation("POST: Create");
        await _mediator.Send(dto.Adapt<CreateUserRequest>());
        // TODO: Handle bad request
        return NoContent();
    }
}
