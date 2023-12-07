using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Alerts;
using SensoBackend.Application.Modules.Alerts.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Alerts;

public sealed class DispatchAlertHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly DispatchAlertHandler _sut;

    private static readonly DateTimeOffset _firedAt = DateTimeOffset.UtcNow;
    private static readonly AlertType _alertType = AlertType.sos;

    public DispatchAlertHandlerTests() => _sut = new DispatchAlertHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldDispatchAlert()
    {
        var account = await _context.SetUpAccount();
        var seniorProfile = await _context.SetUpSeniorProfile(account);

        var request = new DispatchAlertRequest
        {
            Alert = new Alert
            {
                Id = default,
                SeniorId = seniorProfile.SeniorId,
                Type = _alertType,
                FiredAt = _firedAt,
            },
        };

        await _sut.Handle(request, CancellationToken.None);

        var alert = await _context.Alerts.FirstOrDefaultAsync();

        alert.Should().NotBeNull();
        if (alert is null)
            return;

        alert.SeniorId.Should().Be(seniorProfile.SeniorId);
        alert.Type.Should().Be(AlertType.sos);
        alert.FiredAt.Should().Be(_firedAt);
    }
}
