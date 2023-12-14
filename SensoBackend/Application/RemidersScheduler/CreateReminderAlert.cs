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
    IHangfireWrapper hangfireWrapper
) : IRequestHandler<CreateReminderAlertRequest>
{
    private static readonly TimeSpan intakeThreshold = TimeSpan.FromMinutes(5);

    public async Task Handle(CreateReminderAlertRequest request, CancellationToken ct)
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
                Type = AlertType.medicationToTake,
                FiredAt = timeProvider.GetUtcNow(),
            };

            hangfireWrapper.Schedule(
                () => CreateReminderAlert(request.ReminderId, request.SeniorId),
                intakeThreshold
            );

            await alertDispatcher.Dispatch(alert, ct);
        }
    }

    public async Task CreateReminderAlert(int reminderId, int seniorId)
    {
        await mediator.Send(
            new CreateCaretakerReminderAlertRequest { ReminderId = reminderId, SeniorId = seniorId }
        );
    }
}
