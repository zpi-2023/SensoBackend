using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles.CreateCaretakerProfile;

public sealed record CreateCaretakerProfileRequest(CreateCaretakerProfileInternalDto Dto) : IRequest;

[UsedImplicitly]
public sealed class CreateCaretakerProfileValidator : AbstractValidator<CreateCaretakerProfileRequest>
{
    public CreateCaretakerProfileValidator()
    {
        RuleFor(r => r.Dto.AccountId)
            .NotEmpty()
            .WithMessage("AccountId is empty.");
        RuleFor(r => r.Dto.SeniorId)
            .NotEmpty()
            .WithMessage("SeniorId is empty.");
    }
}

[UsedImplicitly]
public sealed class CreateCaretakerProfileHandler : IRequestHandler<CreateCaretakerProfileRequest>
{
    private readonly AppDbContext _context;

    public CreateCaretakerProfileHandler(AppDbContext context) => _context = context;

    public async Task Handle(CreateCaretakerProfileRequest request, CancellationToken ct)
    {
        if(request.Dto.SeniorId == request.Dto.AccountId)
        {
            throw new ValidationException("You cannot create caretaker profile for yourself");
        }

        if (await _context.Profiles.AnyAsync(
            p =>
                p.SeniorId == request.Dto.SeniorId
                && p.AccountId == request.Dto.AccountId,
            ct))
        {
            throw new ValidationException("This caretaker profile already exists");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.Dto.AccountId, ct);
        if (account == null)
        {
            throw new ValidationException($"An account with the given Id ({request.Dto.AccountId}) does not exist");
        }
        var senior = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.Dto.SeniorId, ct);
        if (senior == null)
        {
            throw new ValidationException($"A senior with the given Id ({request.Dto.SeniorId}) does not exist");
        }

        var profile = request.Dto.Adapt<Profile>();
        profile.Alias = request.Dto.SeniorAlias;

        await _context.Profiles.AddAsync(profile, ct);
        await _context.SaveChangesAsync(ct);
    }
}