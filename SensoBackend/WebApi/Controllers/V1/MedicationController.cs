using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Medications;
using SensoBackend.Application.Modules.Medications.Contracts;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class MedicationController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns all medications that fit the search criteria
    /// </summary>
    /// <param name="search"> Search criteria (not case-sensitive) </param>
    /// <response code="200"> Medications that fit the search criteria </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedDto<MedicationDto>))]
    public async Task<IActionResult> GetMedicationList(
        [FromQuery(Name = "search")] string search
    ) => Ok(await mediator.Send(new GetMedicationListRequest { SearchTerm = search }));
}
