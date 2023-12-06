using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Alerts.Contracts;
using SensoBackend.Application.Modules.Alerts.Utils;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Alerts;

public sealed record GetAlertHistoryRequest : IRequest<PaginatedDto<GetAlertHistoryDto>>
{
    public required int AccountId { get; init; }
    public required int SeniorId { get; init; }

    public required PaginationQuery PaginationQuery { get; init; }
    public string? Type { get; init; }
}

[UsedImplicitly]
public sealed class GetAlertHistoryHandler(AppDbContext context)
    : IRequestHandler<GetAlertHistoryRequest, PaginatedDto<GetAlertHistoryDto>>
{
    public async Task<PaginatedDto<GetAlertHistoryDto>> Handle(
        GetAlertHistoryRequest request,
        CancellationToken ct
    )
    {
        var profile =
            await context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == request.AccountId && p.SeniorId == request.SeniorId,
                    ct
                ) ?? throw new ProfileNotFoundException(request.AccountId, request.SeniorId);

        var alerts = request.Type is null
            ? await FindAlerts(profile.SeniorId, request.PaginationQuery, ct)
            : await FindAlertsOfType(
                profile.SeniorId,
                GetAlertType.FromName(request.Type),
                request.PaginationQuery,
                ct
            );

        return new PaginatedDto<GetAlertHistoryDto>
        {
            Items = alerts.Adapt<List<GetAlertHistoryDto>>(),
        };
    }

    private async Task<List<Alert>> FindAlertsOfType(
        int seniorId,
        AlertType alertType,
        PaginationQuery query,
        CancellationToken ct
    )
    {
        return await context
            .Alerts
            .Where(a => a.SeniorId == seniorId && a.Type == alertType)
            .OrderByDescending(a => a.FiredAt)
            .Paged(query)
            .ToListAsync(ct);
    }

    private async Task<List<Alert>> FindAlerts(
        int seniorId,
        PaginationQuery query,
        CancellationToken ct
    )
    {
        return await context
            .Alerts
            .Where(a => a.SeniorId == seniorId)
            .OrderByDescending(a => a.FiredAt)
            .Paged(query)
            .ToListAsync(ct);
    }
}
