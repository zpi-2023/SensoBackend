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
    /// <summary>
    /// Creates an sos alert
    /// </summary>
    /// <response code="204"> Returned when succedded </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="404"> If user has no senior profile </response>
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

    /// <summary>
    /// Returns alerts history for a given senior
    /// </summary>
    /// <param name="seniorId"> The id of a senior </param>
    /// <param name="query"> Pagination query </param>
    /// <param name="type"> Type of alerts to return. When null all alerts types are returned. </param>
    /// <response code="200"> Returns alerts history for a given senior </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="404"> If user has no profile associated with given senior </response>
    [HasPermission(Permission.ReadAlerts)]
    [HttpGet("history/{seniorId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedDto<GetAlertHistoryDto>))]
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
