using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles;

public sealed record GetEncodedSeniorIdRequest : IRequest<EncodedSeniorDto>
{
    public required int AccountId { get; init; }
}

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
        if (
            !(
                await _context
                    .Profiles
                    .AnyAsync(
                        p => p.AccountId == request.AccountId && p.AccountId == p.SeniorId,
                        ct
                    )
            )
        )
        {
            throw new SeniorNotFoundException("Given account does not have a senior profile");
        }

        var account = await _context.Accounts.FirstAsync(a => a.Id == request.AccountId, ct);

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
