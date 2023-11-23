using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.CreateAccount;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class AccountController(ILogger<AccountController> logger, IMediator mediator)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateAccountDto dto)
    {
        logger.LogInformation("Creating new account for {Email}.", dto.Email);
        await mediator.Send(new CreateAccountRequest { Dto = dto });
        return NoContent();
    }

    [HasPermission(Permission.ManageProfiles)]
    [HttpGet("profiles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfilesDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfiles()
    {
        var accountId = this.GetAccountId();
        var profiles = await mediator.Send(
            new GetProfilesByAccountIdRequest { AccountId = accountId }
        );
        return Ok(profiles);
    }

    [HasPermission(Permission.ManageProfiles)]
    [HttpPost("profiles/senior")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfileDisplayDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSeniorProfile()
    {
        var accountId = this.GetAccountId();
        var profile = await mediator.Send(new CreateSeniorProfileRequest { AccountId = accountId });

        return Ok(profile);
    }

    [HasPermission(Permission.ManageProfiles)]
    [HttpGet("profiles/senior")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EncodedSeniorDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSeniorProfile()
    {
        var accountId = this.GetAccountId();
        var encodedData = await mediator.Send(
            new GetEncodedSeniorIdRequest { AccountId = accountId }
        );
        return Ok(encodedData);
    }

    [HasPermission(Permission.ManageProfiles)]
    [HttpDelete("profiles/senior")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSeniorProfile()
    {
        var accountId = this.GetAccountId();
        await mediator.Send(new DeleteSeniorProfileRequest { AccountId = accountId });
        return NoContent();
    }

    [HasPermission(Permission.ManageProfiles)]
    [HttpPost("profiles/caretaker")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfileDisplayDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCaretakerProfile(CreateCaretakerProfileDto dto)
    {
        var accountId = this.GetAccountId();

        var profile = await mediator.Send(
            new CreateCaretakerProfileRequest
            {
                AccountId = accountId,
                Hash = dto.Hash,
                SeniorAlias = dto.SeniorAlias
            }
        );

        return Ok(profile);
    }

    [HasPermission(Permission.ManageProfiles)]
    [HttpDelete("profiles/caretaker/{seniorId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCaretakerProfile(int seniorId)
    {
        var accountId = this.GetAccountId();
        await mediator.Send(
            new DeleteCaretakerProfileRequest { AccountId = accountId, SeniorId = seniorId }
        );
        return NoContent();
    }

    [HasPermission(Permission.ManageProfiles)]
    [HttpPut("profiles/caretaker/{seniorId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfileDisplayDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditCaretakerProfile(int seniorId, EditCaretakerProfileDto dto)
    {
        var accountId = this.GetAccountId();
        var profile = await mediator.Send(
            new EditCaretakerProfileRequest
            {
                AccountId = accountId,
                SeniorId = seniorId,
                SeniorAlias = dto.SeniorAlias
            }
        );
        return Ok(profile);
    }
}
