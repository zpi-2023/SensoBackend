using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Infrastructure.Data;
using System.Security.Authentication;

namespace SensoBackend.Application.Modules.Accounts.GetAccountById;

public sealed record GetAccountByIdRequest : IRequest<AccountDto>
{
    public required GetAccountByIdDto Dto;
}

[UsedImplicitly]
public sealed class GetAccountByIdHandler : IRequestHandler<GetAccountByIdRequest, AccountDto>
{
    private readonly AppDbContext _context;

    public GetAccountByIdHandler(AppDbContext context) => _context = context;

    public async Task<AccountDto> Handle(GetAccountByIdRequest request, CancellationToken ct)
    {
        var account =
            await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.Dto.Id, ct)
            ?? throw new InvalidCredentialException("Account with the given id does not exist");

        return account.Adapt<AccountDto>();
    }
}
