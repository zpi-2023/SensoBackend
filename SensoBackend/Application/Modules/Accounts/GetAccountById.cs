using System.Security.Authentication;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Accounts.GetAccountById;

public sealed record GetAccountByIdRequest : IRequest<AccountDto>
{
    public required GetAccountByIdDto Dto;
}

[UsedImplicitly]
public sealed class GetAccountByIdHandler(AppDbContext context)
    : IRequestHandler<GetAccountByIdRequest, AccountDto>
{
    public async Task<AccountDto> Handle(GetAccountByIdRequest request, CancellationToken ct)
    {
        var account =
            await context.Accounts.FirstOrDefaultAsync(a => a.Id == request.Dto.Id, ct)
            ?? throw new InvalidCredentialException("Account with the given id does not exist");

        return account.Adapt<AccountDto>();
    }
}
