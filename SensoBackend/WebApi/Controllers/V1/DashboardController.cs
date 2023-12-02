using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Dashboard;
using SensoBackend.Application.Modules.Dashboard.Contracts;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class DashboardController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns dashboard for a given senior
    /// </summary>
    /// <param name="seniorId"> Id of a senior </param>
    /// <response code="200"> Returns dashboard for a given senior </response>
    /// <response code="400"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    [HasPermission(Permission.ManageDashboard)]
    [HttpGet("{seniorId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DashboardDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get([FromRoute] int seniorId) =>
        Ok(await mediator.Send(new GetDashboardRequest { SeniorId = seniorId }));

    /// <summary>
    /// Updates dashboard for a given senior
    /// </summary>
    /// <param name="seniorId"> Id of a senior </param>
    /// <param name="dto"> Updated dashboard </param>
    /// <response code="204"> Dashboard successfully updated </response>
    /// <response code="400"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    [HasPermission(Permission.ManageDashboard)]
    [HttpPut("{seniorId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Put([FromRoute] int seniorId, DashboardDto dto)
    {
        await mediator.Send(new UpdateDashboardRequest { SeniorId = seniorId, Dto = dto });
        return NoContent();
    }
}
