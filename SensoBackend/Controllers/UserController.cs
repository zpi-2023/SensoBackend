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
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET: GetAll");
        var users = _service.GetAll();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto newUser)
    {
        _logger.LogInformation("POST: Create");
        var result = _service.Create(newUser);
        return result ? NoContent() : BadRequest();
    }
}
