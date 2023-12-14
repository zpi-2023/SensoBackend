using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Abstractions;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.RemindersScheduler;

public sealed record CreateReminderAlertRequest : IRequest
{
    public required int ReminderId { get; init; }
    public required int SeniorId { get; init; }
}

[UsedImplicitly]
public sealed class CreateReminderAlertHandler(
    AppDbContext context,
    TimeProvider timeProvider,
    IMediator mediator,
    IAlertDispatcher alertDispatcher,
    IHangfireWrapper hangfireWrapper,
    ILogger<CreateReminderAlertHandler> logger
) : IRequestHandler<CreateReminderAlertRequest>
{
    private static readonly TimeSpan intakeThreshold = TimeSpan.FromMinutes(5);

    public async Task Handle(CreateReminderAlertRequest request, CancellationToken ct)
    {
        logger.LogInformation(
            "Creating reminder alert for reminder {ReminderId} and senior {SeniorId}",
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
                Type = AlertType.medicationToTake,
                FiredAt = timeProvider.GetUtcNow(),
            };

            hangfireWrapper.Schedule(
                () => CreateReminderAlert(request.ReminderId, request.SeniorId),
                intakeThreshold
            );

            await alertDispatcher.Dispatch(alert, ct);
        }
        else
        {
            logger.LogInformation(
                "Last intake for reminder {ReminderId} and senior {SeniorId} was less than {IntakeThreshold} ago - not creating alert",
                request.ReminderId,
                request.SeniorId,
                intakeThreshold
            );
        }
    }

    public async Task CreateReminderAlert(int reminderId, int seniorId)
    {
        logger.LogInformation(
            "Sending to mediator request to create alert for caretaker for reminder: {ReminderId}",
            reminderId
        );
        await mediator.Send(
            new CreateCaretakerReminderAlertRequest { ReminderId = reminderId, SeniorId = seniorId }
        );
    }
}
