using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Healthcheck;
using SensoBackend.Application.Modules.Healthcheck.Contracts;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/healthz")]
[ApiVersion("1.0")]
public sealed class HealthcheckController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns the status of the services
    /// </summary>
    /// <response code="200"> Returns status of the services </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HealthcheckDto))]
    public async Task<IActionResult> Get() => Ok(await mediator.Send(new GetHealthRequest()));
}
