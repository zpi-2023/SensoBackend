using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Infrastructure.Data;
using System.Security.Authentication;

namespace SensoBackend.Application.Modules.Accounts.GetAccountById;

public sealed record GetAccountByIdRequest(GetAccountByIdDto Dto) : IRequest<AccountDto>;

[UsedImplicitly]
public sealed class GetAccountByIdHandler : IRequestHandler<GetAccountByIdRequest, AccountDto>
{
    private readonly AppDbContext _context;

    public GetAccountByIdHandler(AppDbContext context) => _context = context;

    public async Task<AccountDto> Handle(GetAccountByIdRequest request, CancellationToken ct)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.Dto.Id, ct);

        if (account == null)
        {
            throw new InvalidCredentialException("Id does not exist");
        }

        return account.Adapt<AccountDto>();
    }
}
