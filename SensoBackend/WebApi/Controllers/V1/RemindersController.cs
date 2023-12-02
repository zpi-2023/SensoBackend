using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Medications;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class RemindersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns reminder with a given Id
    /// </summary>
    /// <param name="reminderId"> Id of a reminder to be returned </param>
    /// <response code="200"> Returns reminder with given id </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this reminder </response>
    /// <response code="404"> If reminder with given Id does not exist in the database </response>
    [HasPermission(Permission.ManageReminders)]
    [HttpGet("{reminderId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReminderDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReminderById(int reminderId)
    {
        var accountId = this.GetAccountId();
        var request = new GetReminderByIdRequest { AccountId = accountId, ReminderId = reminderId };
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Updates reminder with a given Id
    /// </summary>
    /// <param name="reminderId"> Id of a reminder to be returned </param>
    /// <param name="dto"> Data to be updated </param>
    /// <response code="200"> Returns updated reminder </response>
    /// <response code="400"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this reminder </response>
    /// <response code="404"> If reminder with given Id does not exist in the database </response>
    [HasPermission(Permission.ManageReminders)]
    [HttpPut("{reminderId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReminderDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReminderById(int reminderId, UpdateReminderDto dto)
    {
        var accountId = this.GetAccountId();
        var request = new UpdateReminderByIdRequest
        {
            AccountId = accountId,
            ReminderId = reminderId,
            Dto = dto
        };
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Returns intakes for a reminer with a given id
    /// </summary>
    /// <param name="reminderId"> Id of a reminder </param>
    /// <response code="200"> Returns paginated intakes for a reminder with a given id </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this reminder </response>
    /// <response code="404"> If reminder with given Id does not exist in the database </response>
    [HasPermission(Permission.ManageReminders)]
    [HttpGet("{reminderId}/intakes")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedDto<IntakeDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllIntakesForReminder(
        int reminderId,
        [FromQuery] PaginationQuery query
    )
    {
        var accountId = this.GetAccountId();
        var request = new GetAllIntakesForReminderRequest
        {
            AccountId = accountId,
            ReminderId = reminderId,
            PaginationQuery = query
        };
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Deletes reminder with a given id
    /// </summary>
    /// <param name="reminderId"> Id of a reminder </param>
    /// <response code="204"> If operation was successful </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this reminder </response>
    /// <response code="404"> If reminder with given Id does not exist in the database </response>
    [HasPermission(Permission.ManageReminders)]
    [HttpDelete("{reminderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReminderById(int reminderId)
    {
        var accountId = this.GetAccountId();
        var request = new DeleteReminderRequest { AccountId = accountId, ReminderId = reminderId };

        await mediator.Send(request);

        return NoContent();
    }

    /// <summary>
    /// Creates a new intake record for a reminder with a given id
    /// </summary>
    /// <param name="reminderId"> Id of a reminder </param>
    /// <param name="dto"> Information about intake </param>
    /// <response code="201"> Returns newly created intake record </response>
    /// <response code="400"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this reminder </response>
    /// <response code="404"> If reminder with given Id does not exist in the database </response>
    [HasPermission(Permission.ManageReminders)]
    [HttpPost("{reminderId}/intakes")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IntakeDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateIntakeRecord(int reminderId, CreateIntakeDto dto)
    {
        var accountId = this.GetAccountId();
        var request = new CreateIntakeRequest
        {
            AccountId = accountId,
            Dto = dto,
            ReminderId = reminderId
        };

        var intakeDto = await mediator.Send(request);

        return CreatedAtAction(
            nameof(GetIntakeRecordById),
            new { intakeId = intakeDto.Id },
            intakeDto
        );
    }

    /// <summary>
    /// Returns intake with a given id
    /// </summary>
    /// <param name="intakeId"> Id of an intake </param>
    /// <response code="200"> Returns intake with a given id </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this intake </response>
    /// <response code="404"> If intake with given Id does not exist in the database </response>
    [HasPermission(Permission.ManageReminders)]
    [HttpGet("intakes/{intakeId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IntakeDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIntakeRecordById(int intakeId)
    {
        var accountId = this.GetAccountId();
        var request = new GetIntakeByIdRequest { AccountId = accountId, IntakeId = intakeId };
        return Ok(await mediator.Send(request));
    }
}
