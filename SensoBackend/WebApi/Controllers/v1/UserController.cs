using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Users.Contracts;
using SensoBackend.Application.Modules.Users.CreateUser;
using SensoBackend.Application.Modules.Users.GetUsers;

namespace SensoBackend.WebApi.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediator;

    public UserController(ILogger<UserController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<UserDto>))]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET: GetAll");
        return Ok(await _mediator.Send(new GetUsersRequest()));
    }

    [MapToApiVersion("1.0")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        _logger.LogInformation("POST: Create");
        await _mediator.Send(new CreateUserRequest(dto));
        return NoContent();
    }
}
