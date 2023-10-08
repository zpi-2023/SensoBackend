using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Healthcheck.Contracts;
using SensoBackend.Application.Modules.Healthcheck.GetHealth;

namespace SensoBackend.WebApi.Controllers;

[ApiController]
[Route("healthz")]
public class HealthcheckController : ControllerBase
{
    private readonly IMediator _mediator;

    public HealthcheckController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HealthcheckDto))]
    public async Task<IActionResult> Get() => Ok(await _mediator.Send(new GetHealthRequest()));
}
