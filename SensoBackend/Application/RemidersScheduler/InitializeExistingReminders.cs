using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Abstractions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.RemindersScheduler;

public sealed class InitializeExistingReminders(
    IMediator mediator,
    IServiceScopeFactory scopeFactory,
    IHangfireWrapper hangfireWrapper
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var activeReminders = await context
            .Reminders
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var reminder in activeReminders)
        {
            if (reminder.Cron is null)
            {
                continue;
            }

            hangfireWrapper.AddOrUpdate(
                reminder.Id.ToString(),
                () => CreateReminderAlert(reminder.Id, reminder.SeniorId),
                reminder.Cron
            );
        }
    }

    public async Task CreateReminderAlert(int reminderId, int seniorId)
    {
        await mediator.Send(
            new CreateReminderAlertRequest { ReminderId = reminderId, SeniorId = seniorId }
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
