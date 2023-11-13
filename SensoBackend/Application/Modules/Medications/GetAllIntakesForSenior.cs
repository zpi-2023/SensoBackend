using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record GetAllIntakesForSeniorRequest : IRequest<IntakeListDto>
{
    public required int AccountId { get; init; }

    public required int SeniorId { get; init; }
}

[UsedImplicitly]
public sealed class GetAllIntakesForSeniorValidator
    : AbstractValidator<GetAllIntakesForSeniorRequest>
{
    public GetAllIntakesForSeniorValidator()
    {
        RuleFor(r => r.SeniorId)
            .NotEmpty()
            .WithMessage("ReminderId cannot be empty")
            .GreaterThan(0)
            .WithMessage("ReminderId has to be greater than 0");
        RuleFor(r => r.AccountId)
            .NotEmpty()
            .WithMessage("AccountId cannot be empty")
            .GreaterThan(0)
            .WithMessage("AccountId has to be greater than 0");
    }
}

[UsedImplicitly]
public sealed class GetAllIntakesForSeniorHandler
    : IRequestHandler<GetAllIntakesForSeniorRequest, IntakeListDto>
{
    private readonly AppDbContext _context;

    public GetAllIntakesForSeniorHandler(AppDbContext context) => _context = context;

    public async Task<IntakeListDto> Handle(
        GetAllIntakesForSeniorRequest request,
        CancellationToken ct
    )
    {
        var neededProfile =
            await _context.Profiles.FirstOrDefaultAsync(
                p => p.AccountId == request.AccountId && p.SeniorId == request.SeniorId
            ) ?? throw new ReminderAccessDeniedException(request.SeniorId);

        var intakes = await _context.IntakeRecords
            .Include(ir => ir.Reminder)
            .Where(ir => ir.Reminder!.SeniorId == request.SeniorId)
            .ToListAsync();

        var adaptedIntakes = intakes
            .Select(ir => ReminderUtils.AdaptToDto(_context, ir).Result)
            .ToList();

        return new IntakeListDto { Intakes = adaptedIntakes };
    }
}
