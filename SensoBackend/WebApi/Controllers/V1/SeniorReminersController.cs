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
[Route("api/v{version:apiVersion}/reminers/senior")]
[ApiVersion("1.0")]
public sealed class SeniorRemindersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SeniorRemindersController(IMediator mediator) => _mediator = mediator;

    #region SeniorReminders

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
        return Ok(await _mediator.Send(request));
    }

    [HasPermission(Permission.ManageReminders)]
    [HttpPost("{seniorId}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReminderDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateReminder(int seniorId, CreateReminderDto dto)
    {
        var accountId = this.GetAccountId();

        var reminderDto = await _mediator.Send(
            new CreateReminderRequest
            {
                AccountId = accountId,
                SeniorId = seniorId,
                Dto = dto
            }
        );

        return CreatedAtAction(
            actionName: nameof(RemindersController.GetReminderById),
            controllerName: nameof(RemindersController),
            routeValues: new { reminderId = reminderDto.Id },
            value: reminderDto
        );
    }

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
        return Ok(await _mediator.Send(request));
    }

    #endregion SeniorReminders
}
