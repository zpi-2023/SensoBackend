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
public sealed class GetDashboardHandler(AppDbContext context)
    : IRequestHandler<GetDashboardRequest, DashboardDto>
{
    public async Task<DashboardDto> Handle(GetDashboardRequest request, CancellationToken ct)
    {
        var gadgets = await context
            .DashboardItems
            .Where(di => di.AccountId == request.SeniorId)
            .OrderBy(di => di.Position)
            .Select(di => di.Gadget!.ToString("f"))
            .ToListAsync(ct);

        return new DashboardDto { Gadgets = gadgets };
    }
}
