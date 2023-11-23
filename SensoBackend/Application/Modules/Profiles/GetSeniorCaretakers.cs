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
public sealed class GetSeniorCaretakersHandler(AppDbContext context)
    : IRequestHandler<GetSeniorCaretakersRequest, ExtendedProfilesDto>
{
    public async Task<ExtendedProfilesDto> Handle(
        GetSeniorCaretakersRequest request,
        CancellationToken ct
    )
    {
        _ =
            await context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == request.AccountId && p.SeniorId == request.AccountId,
                    ct
                )
            ?? throw new ProfileNotFoundException(
                $"Profile with AccountId {request.AccountId} and SeniorId {request.AccountId} was not found"
            );

        var profiles = await context
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
                        Email = p.Account!.Email,
                        PhoneNumber = p.Account!.PhoneNumber
                    }
            )
            .ToListAsync(ct);

        return profiles.Adapt<ExtendedProfilesDto>();
    }
}
