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
[Route("api/v{version:apiVersion}/reminders/senior")]
[ApiVersion("1.0")]
public sealed class SeniorRemindersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns reminders for a senior with a given id
    /// </summary>
    /// <param name="seniorId"> Id of a senior who's reminders are to be returned </param>
    /// <response code="200"> Returns reminders for a senior with a given id </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this senior's reminders </response>
    [HasPermission(Permission.ManageReminders)]
    [HttpGet("{seniorId}")]
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
    /// Creates a new reminder for a senior with a given Id
    /// </summary>
    /// <param name="seniorId"> Id of a senior </param>
    /// <param name="dto"> Data used to create a new reminder </param>
    /// <response code="201"> Returns newly created reminder </response>
    /// <response code="401"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this senior's reminders </response>
    [HasPermission(Permission.ManageReminders)]
    [HttpPost("{seniorId}")]
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
            actionName: nameof(RemindersController.GetReminderById),
            controllerName: "Reminders",
            routeValues: new { reminderId = reminderDto.Id },
            value: reminderDto
        );
    }

    /// <summary>
    /// Returns intakes for a senior with a given idd
    /// </summary>
    /// <param name="seniorId"> Id of a senior who's reminders are to be returned </param>
    /// <response code="200"> Returns intakes for a senior with a given id </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user does not have a profile needed to access this senior's reminders and their intakes </response>
    [HasPermission(Permission.ManageReminders)]
    [HttpGet("{seniorId}/intakes")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedDto<IntakeDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSeniorIntakes(
        int seniorId,
        [FromQuery] PaginationQuery query
    )
    {
        var accountId = this.GetAccountId();
        var request = new GetAllIntakesForSeniorRequest
        {
            AccountId = accountId,
            SeniorId = seniorId,
            PaginationQuery = query
        };
        return Ok(await mediator.Send(request));
    }
}
