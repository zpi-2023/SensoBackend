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
public sealed class DeleteCaretakerProfileHandler : IRequestHandler<DeleteCaretakerProfileRequest>
{
    private readonly AppDbContext _context;

    public DeleteCaretakerProfileHandler(AppDbContext context) => _context = context;

    public async Task Handle(DeleteCaretakerProfileRequest request, CancellationToken ct)
    {
        var profile =
            await _context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == request.AccountId && p.SeniorId == request.SeniorId,
                    ct
                )
            ?? throw new ProfileNotFoundException(
                $"Profile with AccountId {request.AccountId} and SeniorId {request.SeniorId} was not found"
            );

        _context.Profiles.Remove(profile);
        await _context.SaveChangesAsync(ct);
    }
}
