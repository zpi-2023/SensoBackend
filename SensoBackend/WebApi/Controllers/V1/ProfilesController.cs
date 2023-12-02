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
    /// <summary>
    /// Returns all the profiles for user account
    /// </summary>
    /// <response code="200"> Returns profiles for user's account </response>
    /// <response code="401"> If user is not logged in </response>
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

    /// <summary>
    /// Creates a new senior profile
    /// </summary>
    /// <response code="200"> Returns newly created senior profile </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="409"> If user's account already has a senior profile </response>
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

    /// <summary>
    /// Returns data needed to pair senior with their caretaker
    /// </summary>
    /// <response code="200"> Returns encoded senior data </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="404"> If user's account does not have a senior profile </response>
    [HasPermission(Permission.ManageProfiles)]
    [HttpGet("senior/pairing")]
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

    /// <summary>
    /// Deletes senior profile
    /// </summary>
    /// <response code="204"> Profile was successfully deleted </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If this senior has any caretaker linked to them </response>
    /// <response code="404"> If user's account does not have a senior profile </response>
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

    /// <summary>
    /// Returns all the caretakers for the senior
    /// </summary>
    /// <response code="200"> Returns all of the senior's caretakers </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="404"> If user's account does not have a senior profile </response>
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

    /// <summary>
    /// Creates a new caretaker profile for the user
    /// </summary>
    /// <param name="dto"> Data needed to pair caretaker with the senior </param>
    /// <response code="200"> Returns newly created caretaker profile </response>
    /// <response code="400"> If user tried to create caretaker profile for themself </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="404"> If senior profile user wanted to pair with does not exist </response>
    /// <response code="409"> If user already has a caretaker profile for this senior </response>
    [HasPermission(Permission.ManageProfiles)]
    [HttpPost("caretaker")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfileDisplayDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    /// <summary>
    /// Deletes given caretaker profile
    /// </summary>
    /// <param name="caretakerId"> Id of a caretaker </param>
    /// <param name="seniorId"> Id of a senior </param>
    /// <response code="200"> Returns newly created caretaker profile </response>
    /// <response code="400"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="404"> If given profile was not found </response>
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

    /// <summary>
    /// Updates senior alias in caretaker profile
    /// </summary>
    /// <response code="200"> Returns updated caretaker profile </response>
    /// <response code="400"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="404"> If user's account doesn't have specified caretaker profile </response>
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
