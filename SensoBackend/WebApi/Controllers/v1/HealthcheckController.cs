using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Healthcheck;
using SensoBackend.Application.Modules.Healthcheck.Contracts;

namespace SensoBackend.WebApi.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/healthz")]
[ApiVersion("1.0")]
public class HealthcheckController : ControllerBase
{
    private readonly IMediator _mediator;

    public HealthcheckController(IMediator mediator) => _mediator = mediator;

    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HealthcheckDto))]
    public async Task<IActionResult> Get() => Ok(await _mediator.Send(new GetHealthRequest()));
}
