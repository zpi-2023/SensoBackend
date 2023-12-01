using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.CreateAccount;
using SensoBackend.Application.Modules.Accounts.GetAccountByCredentials;
using SensoBackend.Application.Modules.Token.Contracts;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class AccountController(
    ILogger<AccountController> logger,
    IMediator mediator,
    IJwtProvider jwtProvider
) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreateAccountDto dto)
    {
        logger.LogInformation("Creating new account for {Email}.", dto.Email);
        await mediator.Send(new CreateAccountRequest { Dto = dto });
        return NoContent();
    }

    [HttpPost("token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateToken(GetAccountByCredentialsDto dto)
    {
        logger.LogInformation("Creating new token for {Email}.", dto.Email);
        var account = await mediator.Send(new GetAccountByCredentialsRequest { Dto = dto });
        var token = jwtProvider.GenerateToken(account);
        return Ok(new TokenDto { Token = token, AccountId = account.Id });
    }
}
