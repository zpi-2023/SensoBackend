namespace SensoBackend.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly IConfiguration _configuration;

    public TokenController(ILogger<TokenController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    // Email verification?
    // What should be the restrictions for unverified users?
    // Probably not the lack of access, as this may discourage the use of the mobile app.

    // Login integration with Google?
    // What should be in db table?
    // Its mobile app, how long should be token valid?

    // DB columns ideas:
    // id: int
    // active: bool
    // verified: bool
    // email: string
    // created: Date
    // last_login: Date
    // login: string
    // password: string
    // last_password_change: string
    // phone_number: string ???? should we verify phone number? is it overkill?
    // display_name: string -> should it be here? or in other table? this table should be only for log in correlated things
}
