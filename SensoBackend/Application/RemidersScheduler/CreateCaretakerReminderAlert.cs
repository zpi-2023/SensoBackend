using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Abstractions;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.RemindersScheduler;

public sealed record CreateCaretakerReminderAlertRequest : IRequest
{
    public required int ReminderId { get; init; }
    public required int SeniorId { get; init; }
}

[UsedImplicitly]
public sealed class CreateCaretakerReminderAlertHandler(
    AppDbContext context,
    TimeProvider timeProvider,
    IAlertDispatcher alertDispatcher
) : IRequestHandler<CreateCaretakerReminderAlertRequest>
{
    private static readonly TimeSpan intakeThreshold = TimeSpan.FromMinutes(5);

    public async Task Handle(CreateCaretakerReminderAlertRequest request, CancellationToken ct)
    {
        var lastIntake = await context
            .IntakeRecords
            .Where(r => r.ReminderId == request.ReminderId)
            .OrderByDescending(r => r.TakenAt)
            .FirstOrDefaultAsync(ct);

        if (lastIntake is null || timeProvider.GetUtcNow() - lastIntake.TakenAt > intakeThreshold)
        {
            var alert = new Alert
            {
                Id = default,
                SeniorId = request.SeniorId,
                Type = AlertType.medicationNotTaken,
                FiredAt = timeProvider.GetUtcNow(),
            };

            await alertDispatcher.Dispatch(alert, ct);
        }
    }
}
