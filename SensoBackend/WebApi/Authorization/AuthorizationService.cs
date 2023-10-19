using MediatR;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.GetAccountById;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Domain.Entities;

namespace SensoBackend.WebApi.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly IMediator _mediator;

    public AuthorizationService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<int> GetRoleIdAsync(int accountId)
    {
        var dto = new GetAccountByIdDto { Id = accountId};
        var account = await _mediator.Send(new GetAccountByIdRequest(dto));
        return account.RoleId;
    }

    public async Task<List<ProfileDto>> GetProfilesByAccountId(int accountId)
    {
        var dto = new GetProfilesByAccountIdDto { AccountId = accountId };
        var profiles = await _mediator.Send(new GetProfilesByAccountIdRequest(dto));
        return profiles;
    }
}
