using SensoBackend.Application.Abstractions;

namespace SensoBackend.UnitTests.Utils;

public class MockTimeProvider : ITimeProvider
{
    public DateTimeOffset Now { get; }

    public MockTimeProvider(DateTimeOffset mockNow) => Now = mockNow;
}
