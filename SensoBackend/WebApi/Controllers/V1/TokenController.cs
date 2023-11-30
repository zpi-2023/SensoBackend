using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.GetAccountByCredentials;
using SensoBackend.Application.Modules.Token.Contracts;

namespace SensoBackend.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class TokenController(
    ILogger<TokenController> logger,
    IMediator mediator,
    IJwtProvider jwtProvider
) : ControllerBase
{
    [HttpPost]
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
