using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles;

public sealed record GetSeniorCaretakersRequest : IRequest<ExtendedProfilesDto>
{
    public required int AccountId { get; init; }
}

[UsedImplicitly]
public sealed class GetSeniorCaretakersHandler
    : IRequestHandler<GetSeniorCaretakersRequest, ExtendedProfilesDto>
{
    private readonly AppDbContext _context;

    public GetSeniorCaretakersHandler(AppDbContext context) => _context = context;

    public async Task<ExtendedProfilesDto> Handle(
        GetSeniorCaretakersRequest request,
        CancellationToken ct
    )
    {
        _ =
            await _context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == request.AccountId && p.SeniorId == request.AccountId,
                    ct
                )
            ?? throw new ProfileNotFoundException(
                $"Profile with AccountId {request.AccountId} and SeniorId {request.AccountId} was not found"
            );

        var profiles = await _context
            .Profiles
            .Where(p => p.SeniorId == request.AccountId && p.AccountId != request.AccountId)
            .Include(p => p.Account)
            .Select(
                p =>
                    new ExtendedProfileDto
                    {
                        AccountId = p.AccountId,
                        SeniorId = p.SeniorId,
                        Type = "caretaker",
                        DisplayName = p.Account!.DisplayName,
                        Email = p.Account.Email,
                        PhoneNumber = p.Account!.PhoneNumber
                    }
            )
            .ToListAsync(ct);

        return profiles.Adapt<ExtendedProfilesDto>();
    }
}
