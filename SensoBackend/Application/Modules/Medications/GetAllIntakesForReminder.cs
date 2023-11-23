using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record GetAllIntakesForReminderRequest : IRequest<PaginatedDto<IntakeDto>>
{
    public required int AccountId { get; init; }

    public required int ReminderId { get; init; }

    public required PaginationQuery PaginationQuery { get; init; }
}

[UsedImplicitly]
public sealed class GetAllIntakesForReminderHandler(AppDbContext context)
    : IRequestHandler<GetAllIntakesForReminderRequest, PaginatedDto<IntakeDto>>
{
    public async Task<PaginatedDto<IntakeDto>> Handle(
        GetAllIntakesForReminderRequest request,
        CancellationToken ct
    )
    {
        await ReminderUtils.CheckReminderAndProfile(
            context: context,
            accountId: request.AccountId,
            reminderId: request.ReminderId,
            ct: ct
        );

        var intakes = await context
            .IntakeRecords
            .Where(ir => ir.ReminderId == request.ReminderId)
            .OrderBy(ir => ir.Id)
            .Paged(request.PaginationQuery)
            .ToListAsync(ct);

        var adaptedIntakes = intakes
            .Select(ir => ReminderUtils.AdaptToDto(context, ir).Result)
            .ToList();

        return new PaginatedDto<IntakeDto> { Items = adaptedIntakes };
    }
}
