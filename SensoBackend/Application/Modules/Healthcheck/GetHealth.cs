using JetBrains.Annotations;
using MediatR;
using SensoBackend.Application.Modules.Healthcheck.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Healthcheck;

public sealed record GetHealthRequest : IRequest<HealthcheckDto>;

[UsedImplicitly]
public sealed class GetHealthHandler : IRequestHandler<GetHealthRequest, HealthcheckDto>
{
    private readonly AppDbContext _context;

    public GetHealthHandler(AppDbContext context) => _context = context;

    public async Task<HealthcheckDto> Handle(GetHealthRequest request, CancellationToken ct) =>
        new()
        {
            Server = HealthcheckStatus.Ok,
            Database = await _context.Database.CanConnectAsync(ct)
                ? HealthcheckStatus.Ok
                : HealthcheckStatus.Unhealthy
        };
}
