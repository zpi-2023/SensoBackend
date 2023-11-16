using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Dashboard.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Dashboard;

public sealed record UpdateDashboardRequest : IRequest
{
    public required int SeniorId;
    public required DashboardDto Dto;
}

[UsedImplicitly]
public sealed class UpdateDashboardValidator : AbstractValidator<UpdateDashboardRequest>
{
    public UpdateDashboardValidator()
    {
        RuleFor(r => r.Dto.Gadgets)
            .Must(list => list.Count <= 6)
            .WithMessage("Gadget list is too long.");

        RuleForEach(r => r.Dto.Gadgets)
            .Must(name => Enum.GetValues<Gadget>().Any(g => name == g.ToString("f")))
            .WithMessage("Gadget type is invalid.");
    }
}

[UsedImplicitly]
public sealed class UpdateDashboardHandler : IRequestHandler<UpdateDashboardRequest>
{
    private readonly AppDbContext _context;

    public UpdateDashboardHandler(AppDbContext context) => _context = context;

    public async Task Handle(UpdateDashboardRequest request, CancellationToken ct)
    {
        var gadgetIds = request.Dto.Gadgets.Select(
            (name, position) =>
                (Enum.GetValues<Gadget>().First(g => g.ToString("f") == name), position)
        );

        using var transaction = await _context.Database.BeginTransactionAsync(ct);

        var oldItems = await _context.DashboardItems
            .Where(di => di.AccountId == request.SeniorId)
            .ToListAsync(ct);

        _context.DashboardItems.RemoveRange(oldItems);

        foreach (var (gadget, position) in gadgetIds)
        {
            await _context.DashboardItems.AddAsync(
                new DashboardItem
                {
                    Id = default,
                    AccountId = request.SeniorId,
                    Gadget = gadget,
                    Position = position
                },
                ct
            );
        }

        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }
}
