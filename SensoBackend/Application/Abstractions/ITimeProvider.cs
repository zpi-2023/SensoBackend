namespace SensoBackend.Application.Abstractions;

public interface ITimeProvider
{
    DateTimeOffset Now { get; }
}

internal sealed class TimeProvider : ITimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}
