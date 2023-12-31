using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles;

public sealed record EditCaretakerProfileRequest : IRequest<ProfileDisplayDto>
{
    public required int AccountId { get; init; }

    public required int SeniorId { get; init; }

    public required string SeniorAlias { get; init; }
}

[UsedImplicitly]
public sealed class EditCaretakerProfileValidator : AbstractValidator<EditCaretakerProfileRequest>
{
    public EditCaretakerProfileValidator()
    {
        RuleFor(r => r.AccountId).NotEmpty().WithMessage("AccountId is empty.");
        RuleFor(r => r.SeniorId).NotEmpty().WithMessage("SeniorId is empty.");
        RuleFor(r => r.SeniorAlias).NotEmpty().WithMessage("Hash is empty.");
        RuleFor(r => r.AccountId)
            .NotEqual(r => r.SeniorId)
            .WithMessage("You cannot be your own caretaker.");
    }
}

[UsedImplicitly]
public sealed class EditCaretakerProfileHandler(AppDbContext context)
    : IRequestHandler<EditCaretakerProfileRequest, ProfileDisplayDto>
{
    public async Task<ProfileDisplayDto> Handle(
        EditCaretakerProfileRequest request,
        CancellationToken ct
    )
    {
        var profile =
            await context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == request.AccountId && p.SeniorId == request.SeniorId,
                    ct
                ) ?? throw new ProfileNotFoundException(request.AccountId, request.SeniorId);

        profile.Alias = request.SeniorAlias;
        await context.SaveChangesAsync(ct);
        return new ProfileDisplayDto
        {
            Type = "caretaker",
            SeniorId = profile.SeniorId,
            SeniorAlias = profile.Alias,
        };
    }
}
