using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class ProfilesController(IMediator mediator) : ControllerBase
{
    [HasPermission(Permission.ManageProfiles)]
    [HttpGet]
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
    [HttpPost("senior")]
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
    [HttpGet("senior")]
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
    [HttpDelete("senior")]
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
    [HttpGet("senior/caretakers")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExtendedProfilesDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSeniorCaretakers()
    {
        var accountId = this.GetAccountId();
        var caretakers = await mediator.Send(
            new GetSeniorCaretakersRequest { AccountId = accountId }
        );
        return Ok(caretakers);
    }

    [HasPermission(Permission.ManageProfiles)]
    [HttpPost("caretaker")]
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
    [HttpDelete("caretaker/{seniorId}/{caretakerId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCaretakerProfile(int seniorId, int caretakerId)
    {
        var accountId = this.GetAccountId();
        await mediator.Send(
            new DeleteCaretakerProfileRequest
            {
                AccountId = accountId,
                SeniorId = seniorId,
                CaretakerId = caretakerId
            }
        );
        return NoContent();
    }

    [HasPermission(Permission.ManageProfiles)]
    [HttpPut("caretaker/{seniorId}")]
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
