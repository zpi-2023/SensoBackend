﻿using MediatR;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.GetAccountById;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Enums;

namespace SensoBackend.WebApi.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly IMediator _mediator;

    public AuthorizationService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Role> GetRoleAsync(int accountId)
    {
        var dto = new GetAccountByIdDto { Id = accountId };
        var account = await _mediator.Send(new GetAccountByIdRequest(dto));
        return account.Role;
    }

    public async Task<List<ProfileInfo>> GetProfilesByAccountId(int accountId)
    {
        var profiles = await _mediator.Send(new GetListOfProfilesByAccountIdRequest(accountId));
        return profiles;
    }
}
