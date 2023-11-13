using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Medications;
using SensoBackend.Application.Modules.Medications.Contracts;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class MedicationController : ControllerBase
{
    private readonly IMediator _mediator;

    public MedicationController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MedicationListDto))]
    public async Task<IActionResult> GetMedicationList(
        [FromQuery(Name = "search")] string search
    ) => Ok(await _mediator.Send(new GetMedicationListRequest { SearchTerm = search }));
}
