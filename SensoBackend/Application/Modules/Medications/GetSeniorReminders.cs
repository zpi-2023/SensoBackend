using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record GetSeniorRemindersRequest : IRequest<PaginatedDto<ReminderDto>>
{
    public required int AccountId { get; init; }

    public required int SeniorId { get; init; }

    public required PaginationQuery PaginationQuery { get; init; }
}

[UsedImplicitly]
public sealed class GetSeniorRemindersHandler(AppDbContext context)
    : IRequestHandler<GetSeniorRemindersRequest, PaginatedDto<ReminderDto>>
{
    public async Task<PaginatedDto<ReminderDto>> Handle(
        GetSeniorRemindersRequest request,
        CancellationToken ct
    )
    {
        var neededProfile =
            await context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.AccountId == request.AccountId && p.SeniorId == request.SeniorId
                ) ?? throw new SeniorReminderAccessDeniedException(request.SeniorId);

        var reminders = await context
            .Reminders
            .Where(r => r.SeniorId == request.SeniorId)
            .Include(r => r.Medication)
            .OrderBy(r => r.IsActive ? 0 : 1)
            .ThenBy(r => r.Id)
            .Paged(request.PaginationQuery)
            .ToListAsync(ct);

        var adaptedReminders = reminders
            .Select(r => ReminderUtils.AdaptToDto(context, r).Result)
            .ToList();

        return new PaginatedDto<ReminderDto> { Items = adaptedReminders };
    }
}
