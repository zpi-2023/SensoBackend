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
    /// Returns reminder with a given id
    /// </summary>
    /// <param name="reminderId"> Id of a reminder to be returned </param>
    /// <response code="200"> Returns reminder with given id </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this reminder </response>
    /// <response code="404"> If reminder with given id does not exist in the database </response>
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
    /// Updates reminder with a given id
    /// </summary>
    /// <param name="reminderId"> Id of a reminder to be returned </param>
    /// <param name="dto"> Data to be updated </param>
    /// <response code="200"> Returns updated reminder </response>
    /// <response code="400"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this reminder </response>
    /// <response code="404"> If reminder with given id does not exist in the database </response>
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
    /// Deletes reminder with a given id
    /// </summary>
    /// <param name="reminderId"> Id of a reminder </param>
    /// <response code="204"> If operation was successful </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this reminder </response>
    /// <response code="404"> If reminder with given id does not exist in the database </response>
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
    /// Returns reminders for a senior with a given id
    /// </summary>
    /// <param name="seniorId"> Id of a senior whose reminders are to be returned </param>
    /// <response code="200"> Returns reminders for a senior with a given id </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this senior's reminders </response>
    [HasPermission(Permission.ManageReminders)]
    [HttpGet("senior/{seniorId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedDto<ReminderDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSeniorReminders(
        int seniorId,
        [FromQuery] PaginationQuery query
    )
    {
        var accountId = this.GetAccountId();
        var request = new GetSeniorRemindersRequest
        {
            AccountId = accountId,
            SeniorId = seniorId,
            PaginationQuery = query
        };
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Creates a new reminder for a senior with a given id
    /// </summary>
    /// <param name="seniorId"> Id of a senior </param>
    /// <param name="dto"> Data used to create a new reminder </param>
    /// <response code="201"> Returns newly created reminder </response>
    /// <response code="401"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this senior's reminders </response>
    [HasPermission(Permission.ManageReminders)]
    [HttpPost("senior/{seniorId}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReminderDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateReminder(int seniorId, CreateReminderDto dto)
    {
        var accountId = this.GetAccountId();

        var reminderDto = await mediator.Send(
            new CreateReminderRequest
            {
                AccountId = accountId,
                SeniorId = seniorId,
                Dto = dto
            }
        );

        return CreatedAtAction(
            nameof(GetReminderById),
            new { reminderId = reminderDto.Id },
            reminderDto
        );
    }

    /// <summary>
    /// Returns all medications that fit the search criteria
    /// </summary>
    /// <param name="search"> Search criteria (not case-sensitive) </param>
    /// <response code="200"> Medications that fit the search criteria </response>
    [HttpGet("medications")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedDto<MedicationDto>))]
    public async Task<IActionResult> GetMedicationList(
        [FromQuery(Name = "search")] string search
    ) => Ok(await mediator.Send(new GetMedicationListRequest { SearchTerm = search }));
}
