using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Profiles.Utils;

public sealed class SeniorIdRepoTests
{
    private static readonly SeniorDataToEncode MockSeniorData = new SeniorDataToEncode
    {
        SeniorId = 13,
        SeniorDisplayName = "John",
        ValidTo = new(new DateTime(2022, 3, 13, 15, 37, 1), TimeSpan.Zero)
    };
    private static readonly DateTimeOffset MockNow =
        new(new DateTime(2022, 3, 13, 15, 17, 1), TimeSpan.Zero);

    [Fact]
    public void Get_ShouldReturnData_WhenDataInRepoAndNotExpired()
    {
        var sut = new SeniorIdRepo(new MockTimeProvider { Now = MockNow });
        var hash = sut.AssignHash(MockSeniorData);

        var seniorData = sut.Get(hash);

        seniorData.Should().Be(MockSeniorData);
    }

    [Fact]
    public void Get_ShouldReturnNull_WhenHashIsInvalid()
    {
        var sut = new SeniorIdRepo(new MockTimeProvider { Now = MockNow });

        var seniorData = sut.Get(1234);

        seniorData.Should().Be(null);
    }

    [Fact]
    public void Get_ShouldReturnNull_WhenTheHashHasExpired()
    {
        var timeProvider = new MockTimeProvider { Now = MockNow };
        var sut = new SeniorIdRepo(timeProvider);
        var hash = sut.AssignHash(MockSeniorData);

        timeProvider.Now = timeProvider.Now.AddMinutes(25);
        var seniorData = sut.Get(hash);

        seniorData.Should().Be(null);
    }
}
