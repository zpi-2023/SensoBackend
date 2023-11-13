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

public sealed record GetAllIntakesForReminderRequest : IRequest<IntakeListDto>
{
    public required int AccountId { get; init; }

    public required int ReminderId { get; init; }
}

[UsedImplicitly]
public sealed class GetAllIntakesForReminderValidator
    : AbstractValidator<GetAllIntakesForReminderRequest>
{
    public GetAllIntakesForReminderValidator()
    {
        RuleFor(r => r.ReminderId)
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
public sealed class GetAllIntakesForReminderHandler
    : IRequestHandler<GetAllIntakesForReminderRequest, IntakeListDto>
{
    private readonly AppDbContext _context;

    public GetAllIntakesForReminderHandler(AppDbContext context) => _context = context;

    public async Task<IntakeListDto> Handle(
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

        var intakes = await _context.IntakeRecords
            .Where(ir => ir.ReminderId == request.ReminderId)
            .ToListAsync();

        var adaptedIntakes = intakes
            .Select(ir => ReminderUtils.AdaptToDto(_context, ir).Result)
            .ToList();

        return new IntakeListDto { Intakes = adaptedIntakes };
    }
}
