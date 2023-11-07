using SensoBackend.Application.Abstractions;

namespace SensoBackend.UnitTests.Utils;

public class MockTimeProvider : ITimeProvider
{
    public DateTimeOffset Now { get; set; }
}
