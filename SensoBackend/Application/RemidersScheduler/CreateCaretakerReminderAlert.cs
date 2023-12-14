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
    IAlertDispatcher alertDispatcher,
    ILogger<CreateCaretakerReminderAlertHandler> logger
) : IRequestHandler<CreateCaretakerReminderAlertRequest>
{
    private static readonly TimeSpan intakeThreshold = TimeSpan.FromMinutes(5);

    public async Task Handle(CreateCaretakerReminderAlertRequest request, CancellationToken ct)
    {
        logger.LogInformation(
            "Creating caretaker reminder alert for reminder {ReminderId} and senior {SeniorId}",
            request.ReminderId,
            request.SeniorId
        );

        var lastIntake = await context
            .IntakeRecords
            .Where(r => r.ReminderId == request.ReminderId)
            .OrderByDescending(r => r.TakenAt)
            .FirstOrDefaultAsync(ct);

        logger.LogInformation(
            "Last intake for reminder {ReminderId} and senior {SeniorId} was at {LastIntake}",
            request.ReminderId,
            request.SeniorId,
            lastIntake?.TakenAt
        );

        if (lastIntake is null || timeProvider.GetUtcNow() - lastIntake.TakenAt > intakeThreshold)
        {
            logger.LogInformation(
                "Last intake for reminder {ReminderId} and senior {SeniorId} was more than {IntakeThreshold} ago - creating alert",
                request.ReminderId,
                request.SeniorId,
                intakeThreshold
            );
            var alert = new Alert
            {
                Id = default,
                SeniorId = request.SeniorId,
                Type = AlertType.medicationNotTaken,
                FiredAt = timeProvider.GetUtcNow(),
            };

            await alertDispatcher.Dispatch(alert, ct);
        }
        else
        {
            logger.LogInformation(
                "Last intake for reminder {ReminderId} and senior {SeniorId} was less than {IntakeThreshold} ago - alert not created",
                request.ReminderId,
                request.SeniorId,
                intakeThreshold
            );
        }
    }
}
