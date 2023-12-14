using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.CreateAccount;
using SensoBackend.Application.Modules.Accounts.GetAccountByCredentials;
using SensoBackend.Application.Modules.Accounts.GetAccountById;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

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
    /// <summary>
    /// Creates a new account
    /// </summary>
    /// <param name="dto"> Data needed to create an account </param>
    /// <response code="204"> Account successfully created </response>
    /// <response code="400"> If DTO validation failed </response>
    /// <response code="409"> If an email is already taken </response>
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

    /// <summary>
    /// Returns the id of user's account
    /// </summary>
    /// <response code="200"> Returns the id of user's account </response>
    /// <response code="401"> If user is not logged in </response>
    [HasPermission(Permission.ManageAccount)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAccountDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get()
    {
        var accountId = this.GetAccountId();
        var accountDto = await mediator.Send(new GetAccountRequest { AccountId = accountId });
        return Ok(accountDto);
    }

    /// <summary>
    /// Creates a token for an account to log in
    /// </summary>
    /// <param name="dto"> Credentials </param>
    /// <response code="200"> Returns token </response>
    /// <response code="401"> Credentials are invalid </response>
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

    /// <summary>
    /// Adds a device token to an account
    /// </summary>
    /// <param name="dto"> Device token </param>
    /// <response code="204"> Device token successfully added </response>
    /// <response code="400"> If DTO validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    [HasPermission(Permission.ManageAccount)]
    [HttpPost("device")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddDeviceToken(AddDeviceTokenDto dto)
    {
        var accountId = this.GetAccountId();
        logger.LogInformation("Adding device token for {AccountId}.", accountId);
        await mediator.Send(new AddDeviceTokenRequest { AccountId = accountId, Dto = dto });
        return NoContent();
    }
}
