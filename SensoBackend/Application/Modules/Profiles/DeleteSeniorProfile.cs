using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles;

public sealed record DeleteSeniorProfileRequest : IRequest
{
    public required int AccountId { get; init; }
}

[UsedImplicitly]
public sealed class DeleteSeniorProfileValidator : AbstractValidator<DeleteSeniorProfileRequest>
{
    public DeleteSeniorProfileValidator()
    {
        RuleFor(r => r.AccountId).NotEmpty().WithMessage("AccountId is empty.");
    }
}

[UsedImplicitly]
public sealed class DeleteSeniorProfileHandler(AppDbContext context)
    : IRequestHandler<DeleteSeniorProfileRequest>
{
    public async Task Handle(DeleteSeniorProfileRequest request, CancellationToken ct)
    {
        var profile =
            await context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == request.AccountId && p.SeniorId == request.AccountId,
                    ct
                )
            ?? throw new ProfileNotFoundException(
                $"Profile with AccountId {request.AccountId} and SeniorId {request.AccountId} was not found"
            );

        var hasCaretakerProfiles = await context
            .Profiles
            .AnyAsync(p => p.SeniorId == request.AccountId && p.AccountId != request.AccountId, ct);

        if (hasCaretakerProfiles)
        {
            throw new RemoveSeniorProfileDeniedException(
                "This senior profile has caretaker profiles associated with it"
            );
        }

        context.Profiles.Remove(profile);
        await context.SaveChangesAsync(ct);
    }
}
