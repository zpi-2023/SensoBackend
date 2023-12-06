using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Alerts;
using SensoBackend.Application.Modules.Alerts.Contracts;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class AlertsController(IMediator mediator) : ControllerBase
{
    [HasPermission(Permission.MutateAlerts)]
    [HttpPost("sos")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateSosAlert()
    {
        var accountId = this.GetAccountId();
        await mediator.Send(new CreateSosAlertRequest { AccountId = accountId });
        return NoContent();
    }

    [HasPermission(Permission.ReadAlerts)]
    [HttpGet("history/{seniorId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedDto<GetAlertHistoryDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadHistory(
        int seniorId,
        [FromQuery] PaginationQuery query,
        [FromQuery] string? type
    )
    {
        var accountId = this.GetAccountId();
        var result = await mediator.Send(
            new GetAlertHistoryRequest
            {
                AccountId = accountId,
                SeniorId = seniorId,
                PaginationQuery = query,
                Type = type,
            }
        );
        return Ok(result);
    }
}
