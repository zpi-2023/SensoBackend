using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles.GetEncodedSeniorId;

public sealed record GetEncodedSeniorIdRequest(int AccountId) : IRequest<EncodedSeniorDto>;

[UsedImplicitly]
public sealed class GetEncodedSeniorIdHandler
    : IRequestHandler<GetEncodedSeniorIdRequest, EncodedSeniorDto>
{
    private const int TokenValidForMinutes = 20;

    private readonly AppDbContext _context;
    private readonly ISeniorIdRepo _seniorIdRepo;

    public GetEncodedSeniorIdHandler(AppDbContext context, ISeniorIdRepo seniorIdRepo)
    {
        _context = context;
        _seniorIdRepo = seniorIdRepo;
    }

    public async Task<EncodedSeniorDto> Handle(
        GetEncodedSeniorIdRequest request,
        CancellationToken ct
    )
    {
        var profiles = await _context.Profiles
            .Where(p => p.AccountId == request.AccountId && p.AccountId == p.SeniorId)
            .ToListAsync(ct);

        if (profiles.Count == 0)
        {
            throw new SeniorNotFoundException("Given account does not have a senior profile");
        }

        var account =
            await _context.Accounts.Where(a => a.Id == request.AccountId).FirstOrDefaultAsync(ct)
            ?? throw new AccountNotFoundException("Given account was not found");

        var validTo = DateTimeOffset.UtcNow.AddMinutes(TokenValidForMinutes);
        var seniorData = new SeniorDataToEncode
        {
            SeniorDisplayName = account.DisplayName,
            SeniorId = account.Id,
            ValidTo = validTo
        };

        return new EncodedSeniorDto
        {
            Hash = _seniorIdRepo.AssignHash(seniorData),
            SeniorDisplayName = seniorData.SeniorDisplayName,
            ValidFor = (int)TimeSpan.FromMinutes(TokenValidForMinutes).TotalSeconds
        };
    }
}
