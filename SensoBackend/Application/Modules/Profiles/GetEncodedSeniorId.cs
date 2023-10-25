using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    private static readonly int _tokenValidForMinutes = 20;

    private readonly AppDbContext _context;

    public GetEncodedSeniorIdHandler(AppDbContext context) => _context = context;

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

        var account = await _context.Accounts
            .Where(a => a.Id == request.AccountId)
            .FirstOrDefaultAsync(ct);

        if (account == null)
        {
            throw new AccountNotFoundException("Given account was not found");
        }

        var validTo = DateTime.Now.AddMinutes(_tokenValidForMinutes);
        var seniorData = new SeniorDataToEncode
        {
            SeniorDisplayName = account.DisplayName,
            SeniorId = account.Id,
            ValidTo = validTo
        };
        var hash = SeniorIdRepo.Hash(seniorData);
        SeniorIdRepo.Add(hash, seniorData);

        return new EncodedSeniorDto
        {
            Hash = hash,
            SeniorDisplayName = seniorData.SeniorDisplayName,
            ValidFor = (int)Math.Floor((seniorData.ValidTo - DateTime.Now).TotalSeconds)
        };
    }
}
