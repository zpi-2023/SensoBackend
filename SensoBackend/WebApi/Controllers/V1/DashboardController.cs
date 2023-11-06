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
public sealed class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator) => _mediator = mediator;

    [HasPermission(Permission.ManageDashboard)]
    [HttpGet("{seniorId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DashboardDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get([FromRoute] int seniorId) =>
        Ok(await _mediator.Send(new GetDashboardRequest(seniorId)));

    [HasPermission(Permission.ManageDashboard)]
    [HttpPut("{seniorId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Put([FromRoute] int seniorId, DashboardDto dto)
    {
        await _mediator.Send(new UpdateDashboardRequest(seniorId, dto));
        return NoContent();
    }
}
