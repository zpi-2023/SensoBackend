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
public sealed class RemindersController : ControllerBase
{
    private readonly IMediator _mediator;

    public RemindersController(IMediator mediator) => _mediator = mediator;

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
        return Ok(await _mediator.Send(request));
    }

    [HasPermission(Permission.ManageReminders)]
    [HttpPut("{reminderId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReminderDto))]
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
        return Ok(await _mediator.Send(request));
    }

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
        return Ok(await _mediator.Send(request));
    }

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

        await _mediator.Send(request);

        return NoContent();
    }

    [HasPermission(Permission.ManageReminders)]
    [HttpPost("{reminderId}/intakes")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IntakeDto))]
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

        var intakeDto = await _mediator.Send(request);

        return CreatedAtAction(
            nameof(GetIntakeRecordById),
            new { intakeId = intakeDto.Id },
            intakeDto
        );
    }

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
        return Ok(await _mediator.Send(request));
    }
}
