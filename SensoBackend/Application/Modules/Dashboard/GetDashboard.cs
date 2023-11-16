using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Dashboard.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Dashboard;

public sealed record GetDashboardRequest : IRequest<DashboardDto>
{
    public required int SeniorId;
}

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
            .Select(di => di.Gadget!.ToString("f"))
            .ToListAsync(ct);

        return new DashboardDto { Gadgets = gadgets };
    }
}
