using JetBrains.Annotations;
using MediatR;
using SensoBackend.Application.Modules.Healthcheck.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Healthcheck;

public sealed record GetHealthRequest : IRequest<HealthcheckDto>;

[UsedImplicitly]
public sealed class GetHealthHandler(AppDbContext context)
    : IRequestHandler<GetHealthRequest, HealthcheckDto>
{
    public async Task<HealthcheckDto> Handle(GetHealthRequest request, CancellationToken ct) =>
        new()
        {
            Server = HealthcheckStatus.Ok,
            Database = await context.Database.CanConnectAsync(ct)
                ? HealthcheckStatus.Ok
                : HealthcheckStatus.Unhealthy
        };
}
