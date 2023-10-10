﻿namespace SensoBackend.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.ValidateAccount;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly IMediator _mediator;
    private readonly IJwtProvider _jwtProvider;

    public TokenController(
        ILogger<TokenController> logger,
        IMediator mediator,
        IJwtProvider jwtProvider
    )
    {
        _logger = logger;
        _mediator = mediator;
        _jwtProvider = jwtProvider;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateToken(ValidateAccountDto dto)
    {
        _logger.LogInformation("Creating new token for {Email}.", dto.Email);
        var account = await _mediator.Send(new ValidateAccountRequest(dto));
        return Ok(_jwtProvider.GenerateToken(account));
    }
}
