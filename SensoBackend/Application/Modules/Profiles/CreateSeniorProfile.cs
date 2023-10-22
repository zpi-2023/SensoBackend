using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles;

public sealed record CreateSeniorProfileRequest(CreateSeniorProfileDto Dto) : IRequest;

[UsedImplicitly]
public sealed class CreateSeniorProfileValidator : AbstractValidator<CreateSeniorProfileRequest>
{
    public CreateSeniorProfileValidator()
    {
        RuleFor(r => r.Dto.AccountId)
            .NotEmpty()
            .WithMessage("Id is empty.");
    }
}

[UsedImplicitly]
public sealed class CreateSeniorProfileHandler : IRequestHandler<CreateSeniorProfileRequest>
{
    private readonly AppDbContext _context;

    public CreateSeniorProfileHandler(AppDbContext context) => _context = context;

    public async Task Handle(CreateSeniorProfileRequest request, CancellationToken ct)
    {
        if (await _context.Profiles.AnyAsync(p => p.SeniorId == request.Dto.AccountId, ct))
        {
            throw new ValidationException("This account already has a senior profile");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.Dto.AccountId, ct);
        if (account == null)
        {
            throw new ValidationException($"An account with this given Id {request.Dto.AccountId} does not exist");
        }

        var displayName = account.DisplayName;

        var profile = request.Dto.Adapt<Profile>();
        profile.SeniorId = request.Dto.AccountId;
        profile.Alias = displayName!;

        await _context.Profiles.AddAsync(profile, ct);
        await _context.SaveChangesAsync(ct);
    }
}