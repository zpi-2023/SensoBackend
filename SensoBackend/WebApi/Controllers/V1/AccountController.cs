using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.CreateAccount;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IMediator _mediator;

    public AccountController(ILogger<AccountController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HasPermission(Permission.Write)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(CreateAccountDto dto)
    {
        _logger.LogInformation("Creating new account for {Email}.", dto.Email);
        await _mediator.Send(new CreateAccountRequest(dto));
        return NoContent();
    }
}
