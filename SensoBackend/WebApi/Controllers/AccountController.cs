namespace SensoBackend.WebApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.CreateAccount;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IMediator _mediator;

    public AccountController(ILogger<AccountController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateAccountDto dto)
    {
        _logger.LogInformation("Creating new account for {Email}.", dto.Email);
        await _mediator.Send(new CreateAccountRequest(dto));
        return NoContent();
    }
}
