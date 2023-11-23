using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles;

public sealed record CreateCaretakerProfileRequest : IRequest<ProfileDisplayDto>
{
    public required int AccountId { get; init; }

    public required int Hash { get; init; }

    public required string SeniorAlias { get; init; }
}

[UsedImplicitly]
public sealed class CreateCaretakerProfileHandler(AppDbContext context, ISeniorIdRepo seniorIdRepo)
    : IRequestHandler<CreateCaretakerProfileRequest, ProfileDisplayDto>
{
    public async Task<ProfileDisplayDto> Handle(
        CreateCaretakerProfileRequest request,
        CancellationToken ct
    )
    {
        var seniorData =
            seniorIdRepo.Get(request.Hash)
            ?? throw new SeniorNotFoundException("Provided hash was not found in the database");

        if (seniorData.SeniorId == request.AccountId)
        {
            throw new ValidationException("You cannot create caretaker profile for yourself");
        }

        if (
            await context
                .Profiles
                .AnyAsync(
                    p => p.SeniorId == seniorData.SeniorId && p.AccountId == request.AccountId,
                    ct
                )
        )
        {
            throw new CaretakerProfileAlreadyExistsException(
                "This caretaker profile already exists"
            );
        }

        var account = await context.Accounts.FirstAsync(a => a.Id == request.AccountId, ct);

        var seniorExists = await context
            .Profiles
            .AnyAsync(
                p => p.SeniorId == seniorData.SeniorId && p.AccountId == seniorData.SeniorId,
                ct
            );
        if (!seniorExists)
        {
            throw new SeniorNotFoundException(
                $"A senior with the given Id ({seniorData.SeniorId}) does not exist"
            );
        }

        var profile = request.Adapt<Profile>();
        profile.SeniorId = seniorData.SeniorId;
        profile.Alias = request.SeniorAlias;

        await context.Profiles.AddAsync(profile, ct);
        await context.SaveChangesAsync(ct);

        return new ProfileDisplayDto
        {
            Type = "caretaker",
            SeniorId = profile.SeniorId,
            SeniorAlias = profile.Alias
        };
    }
}
