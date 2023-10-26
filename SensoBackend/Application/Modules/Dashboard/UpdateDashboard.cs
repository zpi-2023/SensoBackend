using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Dashboard.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Dashboard;

public sealed record UpdateDashboardRequest(int SeniorId, DashboardDto Dto) : IRequest;

[UsedImplicitly]
public sealed class UpdateDashboardValidator : AbstractValidator<UpdateDashboardRequest>
{
    public UpdateDashboardValidator()
    {
        RuleFor(r => r.Dto.Gadgets)
            .Must(list => list.Count <= 6)
            .WithMessage("Gadget list is too long.");

        RuleForEach(r => r.Dto.Gadgets)
            .Must(name => Gadget.List.Any(g => name == g.Name))
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
            (name, position) => (Gadget.List.First(g => g.Name == name).Id, position)
        );

        using var transaction = await _context.Database.BeginTransactionAsync(ct);

        var oldItems = await _context.DashboardItems
            .Where(di => di.AccountId == request.SeniorId)
            .ToListAsync(ct);

        _context.DashboardItems.RemoveRange(oldItems);

        foreach (var (gadgetId, position) in gadgetIds)
        {
            await _context.DashboardItems.AddAsync(
                new DashboardItem
                {
                    Id = default,
                    AccountId = request.SeniorId,
                    GadgetId = gadgetId,
                    Position = position
                },
                ct
            );
        }

        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }
}
