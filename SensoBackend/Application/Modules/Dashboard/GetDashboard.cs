using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Dashboard.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Dashboard;

public sealed record GetDashboardRequest(int SeniorId) : IRequest<DashboardDto>;

[UsedImplicitly]
public sealed class GetDashboardHandler : IRequestHandler<GetDashboardRequest, DashboardDto>
{
    private readonly AppDbContext _context;

    public GetDashboardHandler(AppDbContext context) => _context = context;

    public async Task<DashboardDto> Handle(GetDashboardRequest request, CancellationToken ct)
    {
        var gadgets = await _context.DashboardItems
            .Where(di => di.AccountId == request.SeniorId)
            .OrderBy(di => di.Position)
            .Include(di => di.Gadget)
            .Select(di => di.Gadget!.Name)
            .ToListAsync(ct);

        return new DashboardDto { Gadgets = gadgets };
    }
}
