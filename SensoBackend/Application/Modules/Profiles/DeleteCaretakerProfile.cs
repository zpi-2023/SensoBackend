using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles;

public sealed record DeleteCaretakerProfileRequest : IRequest
{
    public required int AccountId { get; init; }

    public required int SeniorId { get; init; }
}

[UsedImplicitly]
public sealed class DeleteCaretakerProfileValidator
    : AbstractValidator<DeleteCaretakerProfileRequest>
{
    public DeleteCaretakerProfileValidator()
    {
        RuleFor(r => r.AccountId).NotEmpty().WithMessage("AccountId is empty.");
        RuleFor(r => r.SeniorId).NotEmpty().WithMessage("SeniorId is empty.");
        RuleFor(r => r.AccountId)
            .NotEqual(r => r.SeniorId)
            .WithMessage("You cannot be your own caretaker.");
    }
}

[UsedImplicitly]
public sealed class DeleteCaretakerProfileHandler(AppDbContext context)
    : IRequestHandler<DeleteCaretakerProfileRequest>
{
    public async Task Handle(DeleteCaretakerProfileRequest request, CancellationToken ct)
    {
        var profile =
            await context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == request.AccountId && p.SeniorId == request.SeniorId,
                    ct
                )
            ?? throw new ProfileNotFoundException(
                $"Profile with AccountId {request.AccountId} and SeniorId {request.SeniorId} was not found"
            );

        context.Profiles.Remove(profile);
        await context.SaveChangesAsync(ct);
    }
}
