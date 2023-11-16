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
public sealed class CreateCaretakerProfileHandler
    : IRequestHandler<CreateCaretakerProfileRequest, ProfileDisplayDto>
{
    private readonly AppDbContext _context;
    private readonly ISeniorIdRepo _seniorIdRepo;

    public CreateCaretakerProfileHandler(AppDbContext context, ISeniorIdRepo seniorIdRepo)
    {
        _context = context;
        _seniorIdRepo = seniorIdRepo;
    }

    public async Task<ProfileDisplayDto> Handle(
        CreateCaretakerProfileRequest request,
        CancellationToken ct
    )
    {
        var seniorData =
            _seniorIdRepo.Get(request.Hash)
            ?? throw new SeniorNotFoundException("Provided hash was not found in the database");

        if (seniorData.SeniorId == request.AccountId)
        {
            throw new ValidationException("You cannot create caretaker profile for yourself");
        }

        if (
            await _context
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

        var account = await _context.Accounts.FirstAsync(a => a.Id == request.AccountId, ct);

        var seniorExists = await _context
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

        await _context.Profiles.AddAsync(profile, ct);
        await _context.SaveChangesAsync(ct);

        return new ProfileDisplayDto
        {
            Type = "caretaker",
            SeniorId = profile.SeniorId,
            SeniorAlias = profile.Alias
        };
    }
}
