using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Alerts.Contracts;
using SensoBackend.Application.Modules.Alerts.Utils;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Alerts;

public sealed record CreateAlertRequest : IRequest
{
    public required int AccountId { get; init; }
    public required CreateAlertDto Dto { get; init; }
}

[UsedImplicitly]
public sealed class CreateAlertHandler(AppDbContext context, TimeProvider timeProvider)
    : IRequestHandler<CreateAlertRequest>
{
    public async Task Handle(CreateAlertRequest request, CancellationToken ct)
    {
        var seniorProfile =
            await context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == request.AccountId && p.SeniorId == request.AccountId,
                    ct
                ) ?? throw new SeniorNotFoundException(request.AccountId);

        var alertType = GetAlertType.FromName(request.Dto.Type);

        var alert = new Alert
        {
            Id = default,
            SeniorId = seniorProfile.SeniorId,
            Type = alertType,
            FiredAt = timeProvider.GetUtcNow(),
        };

        await context.Alerts.AddAsync(alert, ct);
        await context.SaveChangesAsync(ct);
    }
}
