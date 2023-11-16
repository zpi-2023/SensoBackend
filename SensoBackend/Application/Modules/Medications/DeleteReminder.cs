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
        RuleFor(r => r.ReminderId).NotEmpty().WithMessage("ReminderId cannot be empty");
        RuleFor(r => r.AccountId).NotEmpty().WithMessage("AccountId cannot be empty");
    }
}

[UsedImplicitly]
public sealed class DeleteReminderHandler : IRequestHandler<DeleteReminderRequest>
{
    private readonly AppDbContext _context;

    public DeleteReminderHandler(AppDbContext context) => _context = context;

    public async Task Handle(DeleteReminderRequest request, CancellationToken ct)
    {
        var reminder = await ReminderUtils.CheckReminderAndProfile(
            context: _context,
            accountId: request.AccountId,
            reminderId: request.ReminderId,
            ct: ct
        );

        reminder.IsActive = false;
        await _context.SaveChangesAsync(ct);
    }
}
