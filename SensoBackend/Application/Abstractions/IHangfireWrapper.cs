using System.Linq.Expressions;

namespace SensoBackend.Application.Abstractions;

public interface IHangfireWrapper
{
    void AddOrUpdate(
        string recurringJobId,
        Expression<Func<Task>> methodCall,
        string cronExpression
    );
    void RemoveIfExists(string recurringJobId);
    string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay);
}
