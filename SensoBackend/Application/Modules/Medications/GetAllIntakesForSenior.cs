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

public sealed record GetAllIntakesForSeniorRequest : IRequest<PaginatedDto<IntakeDto>>
{
    public required int AccountId { get; init; }

    public required int SeniorId { get; init; }

    public required PaginationQuery PaginationQuery { get; init; }
}

[UsedImplicitly]
public sealed class GetAllIntakesForSeniorHandler
    : IRequestHandler<GetAllIntakesForSeniorRequest, PaginatedDto<IntakeDto>>
{
    private readonly AppDbContext _context;

    public GetAllIntakesForSeniorHandler(AppDbContext context) => _context = context;

    public async Task<PaginatedDto<IntakeDto>> Handle(
        GetAllIntakesForSeniorRequest request,
        CancellationToken ct
    )
    {
        var neededProfile =
            await _context.Profiles.FirstOrDefaultAsync(
                p => p.AccountId == request.AccountId && p.SeniorId == request.SeniorId
            ) ?? throw new SeniorReminderAccessDeniedException(request.SeniorId);

        var intakes = await _context.IntakeRecords
            .Include(ir => ir.Reminder)
            .Where(ir => ir.Reminder!.SeniorId == request.SeniorId)
            .OrderBy(ir => ir.Id)
            .Paged(request.PaginationQuery)
            .ToListAsync(ct);

        var adaptedIntakes = intakes
            .Select(ir => ReminderUtils.AdaptToDto(_context, ir).Result)
            .ToList();

        return new PaginatedDto<IntakeDto> { Items = adaptedIntakes };
    }
}
