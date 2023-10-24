using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.CreateAccount;
using SensoBackend.Application.Modules.Profiles.CreateSeniorProfile;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Application.Modules.Profiles.GetProfilesByAccountId;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;
using SensoBackend.Application.Modules.Profiles.CreateCaretakerProfile;
using SensoBackend.Application.Modules.Profiles.GetEncodedSeniorId;

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

    [HasPermission(Permission.ProfileAccess)]
    [HttpGet("profiles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfilesDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfiles()
    {
        var accountId = this.GetAccountId();
        var profiles = await _mediator.Send(new GetProfilesByAccountIdRequest(accountId));
        return Ok(profiles);
    }

    [HasPermission(Permission.ProfileAccess)]
    [HttpPost("profiles/senior")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSeniorProfile()
    {
        var accountId = this.GetAccountId();
        await _mediator.Send(new CreateSeniorProfileRequest(accountId));

        return NoContent();
    }

    [HasPermission(Permission.ProfileAccess)]
    [HttpGet("profiles/senior")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EncodedSeniorDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSeniorProfile()
    {
        var accountId = this.GetAccountId();
        var encodedData = await _mediator.Send(new GetEncodedSeniorIdRequest(accountId));
        return Ok(encodedData);
    }

    [HasPermission(Permission.ProfileAccess)]
    [HttpPost("profiles/caretaker")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCaretakerProfile(CreateCaretakerProfileDto dto)
    {
        var accountId = this.GetAccountId();

        await _mediator.Send(
            new CreateCaretakerProfileRequest
            {
                AccountId = accountId,
                EncodedSeniorId = dto.EncodedSeniorId,
                SeniorAlias = dto.SeniorAlias
            });

        return NoContent();
    }
}
