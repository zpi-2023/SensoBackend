using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Exceptions;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles.CreateCaretakerProfile;

public sealed record CreateCaretakerProfileRequest : IRequest
{
    public required int AccountId { get; init; }

    public required int EncodedSeniorId { get; init; }

    public required string SeniorAlias { get; init; }
}

[UsedImplicitly]
public sealed class CreateCaretakerProfileValidator : AbstractValidator<CreateCaretakerProfileRequest>
{
    public CreateCaretakerProfileValidator()
    {
        RuleFor(r => r.AccountId)
            .NotEmpty()
            .WithMessage("AccountId is empty.");
        RuleFor(r => r.EncodedSeniorId)
            .NotEmpty()
            .WithMessage("EncodedSeniorId is empty.");
    }
}

[UsedImplicitly]
public sealed class CreateCaretakerProfileHandler : IRequestHandler<CreateCaretakerProfileRequest>
{
    private readonly AppDbContext _context;

    public CreateCaretakerProfileHandler(AppDbContext context) => _context = context;

    public async Task Handle(CreateCaretakerProfileRequest request, CancellationToken ct)
    {
        var seniorData = SeniorIdRepo.Get(request.EncodedSeniorId);

        if (seniorData == null)
        {
            throw new SeniorNotFoundException("Provided hash was not found in the database");
        }

        if(seniorData.SeniorId == request.AccountId)
        {
            throw new ValidationException("You cannot create caretaker profile for yourself");
        }

        if (await _context.Profiles.AnyAsync(
            p =>
                p.SeniorId == seniorData.SeniorId
                && p.AccountId == request.AccountId,
            ct))
        {
            throw new CaretakerProfileAlreadyExistsException("This caretaker profile already exists");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId, ct);
        if (account == null)
        {
            throw new AccountNotFoundException($"An account with the given Id ({request.AccountId}) does not exist");
        }
        var senior = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == seniorData.SeniorId, ct);
        if (senior == null)
        {
            throw new SeniorNotFoundException($"A senior with the given Id ({seniorData.SeniorId}) does not exist");
        }

        var profile = request.Adapt<Profile>();
        profile.SeniorId = seniorData.SeniorId;
        profile.Alias = request.SeniorAlias;

        await _context.Profiles.AddAsync(profile, ct);
        await _context.SaveChangesAsync(ct);
    }
}