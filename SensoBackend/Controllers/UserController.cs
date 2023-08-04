using Microsoft.AspNetCore.Mvc;
using SensoBackend.Contracts.User;
using SensoBackend.Services;

namespace SensoBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _service;

    public UserController(ILogger<UserController> logger, IUserService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET: GetAll");
        var users = _service.GetAll();
        return Ok(users);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateUserDto newUser)
    {
        _logger.LogInformation("POST: Create");
        var result = _service.Create(newUser);
        return result ? NoContent() : BadRequest();
    }
}
