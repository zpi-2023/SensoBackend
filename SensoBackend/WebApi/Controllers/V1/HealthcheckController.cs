using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Healthcheck.Contracts;
using SensoBackend.Application.Modules.Healthcheck;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/healthz")]
[ApiVersion("1.0")]
public class HealthcheckController : ControllerBase
{
    private readonly IMediator _mediator;

    public HealthcheckController(IMediator mediator) => _mediator = mediator;

    [HasPermission(Permission.AdminAccess)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HealthcheckDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get() => Ok(await _mediator.Send(new GetHealthRequest()));
}
