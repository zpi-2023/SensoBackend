using JetBrains.Annotations;
using MediatR;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Alerts;

public sealed record DispatchAlertRequest : IRequest
{
    public required Alert Alert { get; init; }
}

[UsedImplicitly]
public sealed class DispatchAlertHandler(AppDbContext context)
    : IRequestHandler<DispatchAlertRequest>
{
    public async Task Handle(DispatchAlertRequest request, CancellationToken ct)
    {
        await context.Alerts.AddAsync(request.Alert, ct);
        await context.SaveChangesAsync(ct);

        // TODO: send push notifications
    }
}
