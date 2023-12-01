using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Accounts.GetAccountById;

public sealed record GetAccountRequest : IRequest<GetAccountDto>
{
    public required int AccountId { get; init; }
}

[UsedImplicitly]
public sealed class GetAccountHandler(AppDbContext context)
    : IRequestHandler<GetAccountRequest, GetAccountDto>
{
    public async Task<GetAccountDto> Handle(GetAccountRequest request, CancellationToken ct)
    {
        var account = await context
            .Accounts
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, ct);

        return account.Adapt<GetAccountDto>();
    }
}
