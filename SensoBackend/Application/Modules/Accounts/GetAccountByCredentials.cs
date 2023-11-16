using System.Security.Authentication;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Accounts.GetAccountByCredentials;

public sealed record GetAccountByCredentialsRequest : IRequest<AccountDto>
{
    public required GetAccountByCredentialsDto Dto;
}

[UsedImplicitly]
public sealed class GetAccountByCredentialsHandler
    : IRequestHandler<GetAccountByCredentialsRequest, AccountDto>
{
    private readonly AppDbContext _context;

    public GetAccountByCredentialsHandler(AppDbContext context) => _context = context;

    public async Task<AccountDto> Handle(
        GetAccountByCredentialsRequest request,
        CancellationToken ct
    )
    {
        var account = await _context
            .Accounts
            .FirstOrDefaultAsync(a => a.Email == request.Dto.Email, ct);

        if (account == null || !BCrypt.Net.BCrypt.Verify(request.Dto.Password, account.Password))
        {
            throw new InvalidCredentialException("Email or password is incorrect");
        }

        return account.Adapt<AccountDto>();
    }
}
