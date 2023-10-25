using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles.CreateSeniorProfile;

public sealed record CreateSeniorProfileRequest(int AccountId) : IRequest;

[UsedImplicitly]
public sealed class CreateSeniorProfileValidator : AbstractValidator<CreateSeniorProfileRequest>
{
    public CreateSeniorProfileValidator()
    {
        RuleFor(r => r.AccountId).NotEmpty().WithMessage("Id is empty.");
    }
}

[UsedImplicitly]
public sealed class CreateSeniorProfileHandler : IRequestHandler<CreateSeniorProfileRequest>
{
    private readonly AppDbContext _context;

    public CreateSeniorProfileHandler(AppDbContext context) => _context = context;

    public async Task Handle(CreateSeniorProfileRequest request, CancellationToken ct)
    {
        if (await _context.Profiles.AnyAsync(p => p.SeniorId == request.AccountId, ct))
        {
            throw new ValidationException("This account already has a senior profile");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(
            a => a.Id == request.AccountId,
            ct
        );
        if (account == null)
        {
            throw new AccountNotFoundException(
                $"An account with the given Id ({request.AccountId}) does not exist"
            );
        }

        var displayName = account.DisplayName;

        var profile = request.Adapt<Profile>();
        profile.Alias = displayName;
        profile.SeniorId = request.AccountId;

        await _context.Profiles.AddAsync(profile, ct);
        await _context.SaveChangesAsync(ct);
    }
}
