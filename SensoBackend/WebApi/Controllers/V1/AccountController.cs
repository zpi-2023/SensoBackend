using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.CreateAccount;
using SensoBackend.Application.Modules.Profiles.CreateSeniorProfile;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Application.Modules.Profiles.GetListOfProfilesByAccountId;
using SensoBackend.Application.Modules.Profiles.GetProfilesByAccountId;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;
using SensoBackend.Application.Modules.Profiles.CreateCaretakerProfile;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AccountController : ControllerBase
{
    private const string dziadEncryptionKey = "SuperSecureSeniorSensoKey";
    private EncryptionService _encryptionService = new EncryptionService();

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

    [HasPermission(Permission.CreateProfile)]
    [HttpPost("profiles/senior")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSeniorProfile()
    {
        var accountId = this.GetAccountId();

        var existingProfilesDto = new GetProfilesByAccountIdDto { AccountId = accountId };

        var profiles = await _mediator.Send(new GetListOfProfilesByAccountIdRequest(existingProfilesDto));
        var seniorProfile = profiles.FirstOrDefault(p => p.AccountId == p.SeniorId);
        if(seniorProfile != null)
        {
            return Conflict("Account already has a senior profile");
        }

        var dto = new CreateSeniorProfileDto { AccountId = accountId };
        await _mediator.Send(new CreateSeniorProfileRequest(dto));

        return NoContent();
    }

    [HasPermission(Permission.AccessProfiles)]
    [HttpGet("profiles/senior")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSeniorProfile()
    {
        var accountId = this.GetAccountId();
        var dto = new GetProfilesByAccountIdDto { AccountId = accountId };

        var profiles = await _mediator.Send(new GetListOfProfilesByAccountIdRequest(dto));
        var isSenior = profiles.Any(p => p.AccountId == p.SeniorId);

        if (!isSenior)
        {
            return NotFound();
        }

        var encodedSeniorId = await _encryptionService.EncryptAsync(accountId.ToString(), dziadEncryptionKey);

        return Ok(encodedSeniorId);
    }

    [HasPermission(Permission.CreateProfile)]
    [HttpPost("profiles/caretaker")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCaretakerProfile(CreateCaretakerProfileDto dto)
    {
        var accountId = this.GetAccountId();

        var seniorIdStr = await _encryptionService.DecryptAsync(dto.EncodedSeniorId, dziadEncryptionKey);
        var isString = Int32.TryParse(seniorIdStr, out var seniorId);

        if (!isString)
        {
            return Forbid();
        }

        var internalDto = new CreateCaretakerProfileInternalDto
        {
            AccountId = accountId,
            SeniorId = seniorId,
            SeniorAlias = dto.SeniorAlias
        };
        await _mediator.Send(new CreateCaretakerProfileRequest(internalDto));
        return NoContent();
    }
}
