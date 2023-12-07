using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Alerts;

public sealed record CreateSosAlertRequest : IRequest
{
    public required int AccountId { get; init; }
}

[UsedImplicitly]
public sealed class CreateSosAlertHandler(
    AppDbContext context,
    TimeProvider timeProvider,
    IMediator mediator
) : IRequestHandler<CreateSosAlertRequest>
{
    public async Task Handle(CreateSosAlertRequest request, CancellationToken ct)
    {
        var seniorProfile =
            await context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == request.AccountId && p.SeniorId == request.AccountId,
                    ct
                ) ?? throw new SeniorNotFoundException(request.AccountId);

        var alert = new Alert
        {
            Id = default,
            SeniorId = seniorProfile.SeniorId,
            Type = AlertType.sos,
            FiredAt = timeProvider.GetUtcNow(),
        };

        await mediator.Send(new DispatchAlertRequest { Alert = alert }, ct);
    }
}
