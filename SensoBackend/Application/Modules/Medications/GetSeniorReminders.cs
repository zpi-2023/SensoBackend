using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record GetSeniorRemindersRequest : IRequest<ReminderListDto>
{
    public required int AccountId { get; init; }

    public required int SeniorId { get; init; }
}

[UsedImplicitly]
public sealed class GetSeniorRemindersValidator : AbstractValidator<GetSeniorRemindersRequest>
{
    public GetSeniorRemindersValidator()
    {
        RuleFor(r => r.SeniorId)
            .NotEmpty()
            .WithMessage("SeniorId cannot be empty")
            .GreaterThan(0)
            .WithMessage("SeniorId has to be greater than 0");
        RuleFor(r => r.AccountId)
            .NotEmpty()
            .WithMessage("AccountId cannot be empty")
            .GreaterThan(0)
            .WithMessage("AccountId has to be greater than 0");
    }
}

[UsedImplicitly]
public sealed class GetSeniorRemindersHandler : IRequestHandler<GetSeniorRemindersRequest, ReminderListDto>
{
    private readonly AppDbContext _context;

    public GetSeniorRemindersHandler(AppDbContext context) => _context = context;

    public async Task<ReminderListDto> Handle(GetSeniorRemindersRequest request, CancellationToken ct)
    {
        var neededProfile = await _context.Profiles
            .FirstOrDefaultAsync(
                p =>
                    p.AccountId == request.AccountId
                    && p.SeniorId == request.SeniorId
            ) ?? throw new SeniorReminderAccessDeniedException(request.SeniorId);

        var reminders = await _context.Reminders
            .Where(r => r.SeniorId == request.SeniorId)
            .Include(r => r.Medication)
            .ToListAsync(ct);

        var adaptedReminders = reminders
            .Select(r => ReminderUtils.AdaptToDto(_context, r).Result)
            .ToList();

        return new ReminderListDto { Reminders = adaptedReminders };
    }
}
