using JetBrains.Annotations;
using MediatR;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record DeleteReminderRequest : IRequest
{
    public required int AccountId { get; init; }

    public required int ReminderId { get; init; }
}

[UsedImplicitly]
public sealed class DeleteReminderHandler(AppDbContext context, IHangfireWrapper hangfireWrapper)
    : IRequestHandler<DeleteReminderRequest>
{
    public async Task Handle(DeleteReminderRequest request, CancellationToken ct)
    {
        var reminder = await ReminderUtils.CheckReminderAndProfile(
            context: context,
            accountId: request.AccountId,
            reminderId: request.ReminderId,
            ct: ct
        );

        reminder.IsActive = false;
        await context.SaveChangesAsync(ct);

        hangfireWrapper.RemoveIfExists(reminder.Id.ToString());
    }
}
