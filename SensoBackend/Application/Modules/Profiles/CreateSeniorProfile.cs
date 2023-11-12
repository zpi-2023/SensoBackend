using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Application.Modules.Dashboard;
using SensoBackend.Application.Modules.Dashboard.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles.CreateSeniorProfile;

public sealed record CreateSeniorProfileRequest(int AccountId) : IRequest<ProfileDisplayDto>;

[UsedImplicitly]
public sealed class CreateSeniorProfileHandler
    : IRequestHandler<CreateSeniorProfileRequest, ProfileDisplayDto>
{
    private readonly AppDbContext _context;
    private readonly IMediator _mediator;

    public CreateSeniorProfileHandler(AppDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<ProfileDisplayDto> Handle(
        CreateSeniorProfileRequest request,
        CancellationToken ct
    )
    {
        if (await _context.Profiles.AnyAsync(p => p.SeniorId == request.AccountId, ct))
        {
            throw new SeniorProfileAlreadyExistsException(
                "This account already has a senior profile"
            );
        }

        var account =
            await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId, ct)
            ?? throw new AccountNotFoundException(
                $"An account with the given Id ({request.AccountId}) does not exist"
            );

        var displayName = account.DisplayName;

        var profile = request.Adapt<Profile>();
        profile.Alias = displayName;
        profile.SeniorId = request.AccountId;

        await _context.Profiles.AddAsync(profile, ct);
        await _context.SaveChangesAsync(ct);

        await _mediator.Send(
            new UpdateDashboardRequest(
                account.Id,
                new DashboardDto
                {
                    Gadgets = new()
                    {
                        "trackMedication",
                        "manageNotes",
                        "playGames",
                        "editDashboard"
                    }
                }
            ),
            ct
        );

        return new ProfileDisplayDto
        {
            Type = "senior",
            SeniorId = profile.SeniorId,
            SeniorAlias = profile.Alias
        };
    }
}
