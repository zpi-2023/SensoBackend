using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record DeleteReminderRequest : IRequest
{
    public required int AccountId { get; init; }

    public required int ReminderId { get; init; }
}

[UsedImplicitly]
public sealed class DeleteReminderValidator : AbstractValidator<DeleteReminderRequest>
{
    public DeleteReminderValidator()
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
public sealed class DeleteReminderHandler : IRequestHandler<DeleteReminderRequest>
{
    private readonly AppDbContext _context;

    public DeleteReminderHandler(AppDbContext context) => _context = context;

    public async Task Handle(DeleteReminderRequest request, CancellationToken ct)
    {
        var reminder =
            await _context.Reminders.FindAsync(request.ReminderId, ct)
            ?? throw new ReminderNotFoundException(request.ReminderId);

        var neededProfile =
            await _context.Profiles.FirstOrDefaultAsync(
                p => p.AccountId == request.AccountId && p.SeniorId == reminder.SeniorId,
                ct
            ) ?? throw new ReminderAccessDeniedException(request.ReminderId);

        reminder.IsActive = false;
        await _context.SaveChangesAsync(ct);
    }
}
