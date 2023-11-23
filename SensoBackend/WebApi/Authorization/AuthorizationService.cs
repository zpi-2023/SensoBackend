using MediatR;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.GetAccountById;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Enums;

namespace SensoBackend.WebApi.Authorization;

public class AuthorizationService(IMediator mediator) : IAuthorizationService
{
    public async Task<Role> GetRoleAsync(int accountId)
    {
        var dto = new GetAccountByIdDto { Id = accountId };
        var account = await mediator.Send(new GetAccountByIdRequest { Dto = dto });
        return account.Role;
    }

    public async Task<List<ProfileInfo>> GetProfilesByAccountId(int accountId)
    {
        var profiles = await mediator.Send(
            new GetListOfProfilesByAccountIdRequest { AccountId = accountId }
        );
        return profiles;
    }
}
