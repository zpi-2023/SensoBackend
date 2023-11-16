﻿using FluentValidation;
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
public sealed class GetAllIntakesForReminderHandler
    : IRequestHandler<GetAllIntakesForReminderRequest, PaginatedDto<IntakeDto>>
{
    private readonly AppDbContext _context;

    public GetAllIntakesForReminderHandler(AppDbContext context) => _context = context;

    public async Task<PaginatedDto<IntakeDto>> Handle(
        GetAllIntakesForReminderRequest request,
        CancellationToken ct
    )
    {
        await ReminderUtils.CheckReminderAndProfile(
            context: _context,
            accountId: request.AccountId,
            reminderId: request.ReminderId,
            ct: ct
        );

        var intakes = await _context
            .IntakeRecords
            .Where(ir => ir.ReminderId == request.ReminderId)
            .OrderBy(ir => ir.Id)
            .Paged(request.PaginationQuery)
            .ToListAsync(ct);

        var adaptedIntakes = intakes
            .Select(ir => ReminderUtils.AdaptToDto(_context, ir).Result)
            .ToList();

        return new PaginatedDto<IntakeDto> { Items = adaptedIntakes };
    }
}
