namespace SensoBackend.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly IConfiguration _configuration;

    private static readonly TimeSpan TokenLifetime = TimeSpan.FromDays(7);

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
}
