using System.Linq.Expressions;
using Hangfire;
using SensoBackend.Application.Abstractions;

namespace SensoBackend.Application.RemindersScheduler;

public sealed class HangfireWrapper : IHangfireWrapper
{
    public void AddOrUpdate(
        string recurringJobId,
        Expression<Func<Task>> methodCall,
        string cronExpression
    )
    {
        RecurringJob.AddOrUpdate(
            recurringJobId,
            methodCall,
            cronExpression,
            new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc }
        );
    }

    public void RemoveIfExists(string recurringJobId)
    {
        RecurringJob.RemoveIfExists(recurringJobId);
    }

    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }
}
