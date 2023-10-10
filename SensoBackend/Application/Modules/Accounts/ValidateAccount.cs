using System.Security.Authentication;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Accounts.ValidateAccount;

public sealed record ValidateAccountRequest(ValidateAccountDto Dto) : IRequest<AccountDto>;

[UsedImplicitly]
public sealed class ValidateAccountHandler : IRequestHandler<ValidateAccountRequest, AccountDto>
{
    private readonly AppDbContext _context;

    public ValidateAccountHandler(AppDbContext context) => _context = context;

    public async Task<AccountDto> Handle(ValidateAccountRequest request, CancellationToken ct)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(
            a => a.Email == request.Dto.Email,
            ct
        );

        if (account == null || !BCrypt.Net.BCrypt.Verify(request.Dto.Password, account.Password))
        {
            throw new InvalidCredentialException("Email or password is incorrect");
        }

        return account.Adapt<AccountDto>();
    }
}
