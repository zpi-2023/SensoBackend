using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.CreateAccount;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Application.Modules.Profiles.GetProfilesByAccountId;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IMediator _mediator;

    public AccountController(ILogger<AccountController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateAccountDto dto)
    {
        _logger.LogInformation("Creating new account for {Email}.", dto.Email);
        await _mediator.Send(new CreateAccountRequest(dto));
        return NoContent();
    }

    [HasPermission(Permission.AccessProfiles)]
    [HttpGet("profiles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfiles()
    {
        var accountId = this.GetAccountId();
        var dto = new GetProfilesByAccountIdDto { AccountId = accountId };

        var profiles = await _mediator.Send(new GetProfilesByAccountIdRequest(dto));
        return Ok(profiles);
    }
}
