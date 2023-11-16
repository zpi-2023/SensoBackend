using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles;

public sealed record DeleteSeniorProfileRequest(int AccountId) : IRequest;

[UsedImplicitly]
public sealed class DeleteSeniorProfileValidator : AbstractValidator<DeleteSeniorProfileRequest>
{
    public DeleteSeniorProfileValidator()
    {
        RuleFor(r => r.AccountId).NotEmpty().WithMessage("AccountId is empty.");
    }
}

[UsedImplicitly]
public sealed class DeleteSeniorProfileHandler : IRequestHandler<DeleteSeniorProfileRequest>
{
    private readonly AppDbContext _context;

    public DeleteSeniorProfileHandler(AppDbContext context) => _context = context;

    public async Task Handle(DeleteSeniorProfileRequest request, CancellationToken ct)
    {
        var profile =
            await _context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == request.AccountId && p.SeniorId == request.AccountId,
                    ct
                )
            ?? throw new ProfileNotFoundException(
                $"Profile with AccountId {request.AccountId} and SeniorId {request.AccountId} was not found"
            );

        var hasCaretakerProfiles = await _context
            .Profiles
            .AnyAsync(p => p.SeniorId == request.AccountId && p.AccountId != request.AccountId, ct);

        if (hasCaretakerProfiles)
        {
            throw new RemoveSeniorProfileDeniedException(
                "This senior profile has caretaker profiles associated with it"
            );
        }

        _context.Profiles.Remove(profile);
        await _context.SaveChangesAsync(ct);
    }
}
