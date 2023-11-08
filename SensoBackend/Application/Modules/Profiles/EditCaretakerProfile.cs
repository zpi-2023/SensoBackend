using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles.EditCaretakerProfile;

public sealed record EditCaretakerProfileRequest : IRequest
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
    }
}

[UsedImplicitly]
public sealed class EditCaretakerProfileHandler : IRequestHandler<EditCaretakerProfileRequest>
{
    private readonly AppDbContext _context;

    public EditCaretakerProfileHandler(AppDbContext context) => _context = context;

    public async Task Handle(EditCaretakerProfileRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
