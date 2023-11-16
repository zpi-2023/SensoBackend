using Microsoft.Extensions.Time.Testing;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Profiles;

public sealed class GetEncodedSeniorIdHandlerTests
{
    private readonly AppDbContext _context = Database.CreateFixture();

    private readonly GetEncodedSeniorIdHandler _sut;

    public GetEncodedSeniorIdHandlerTests() =>
        _sut = new GetEncodedSeniorIdHandler(
            _context,
            new SeniorIdRepo(
                new FakeTimeProvider(new(2021, 6, 15, 10, 0, 0, TimeSpan.Zero))
            )
        );

    [Fact]
    public async Task Handle_ShouldThrow_WhenNoSeniorProfileExists()
    {
        var account = await _context.SetUpAccount();

        var action = async () =>
            await _sut.Handle(new GetEncodedSeniorIdRequest(account.Id), CancellationToken.None);

        await action.Should().ThrowAsync<SeniorNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldReturnDto_WhenSeniorProfileExists()
    {
        var account = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(account);

        var dto = await _sut.Handle(
            new GetEncodedSeniorIdRequest(account.Id),
            CancellationToken.None
        );

        dto.SeniorDisplayName.Should().Be(account.DisplayName);
        dto.ValidFor.Should().Be(1200);
    }
}
