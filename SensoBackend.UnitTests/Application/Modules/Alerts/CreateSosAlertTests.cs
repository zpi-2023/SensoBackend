using MediatR;
using Microsoft.Extensions.Time.Testing;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Alerts;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Alerts;

public sealed class CreateSosAlertHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly CreateSosAlertHandler _sut;
    private readonly IAlertDispatcher _alertDispatcher = Substitute.For<IAlertDispatcher>();

    private static readonly DateTimeOffset _firedAt = DateTimeOffset.UtcNow;

    private static readonly AlertType[] _alertTypes =
    [
        AlertType.sos,
        AlertType.sos,
        AlertType.medicationNotTaken,
        AlertType.medicationToTake,
        AlertType.sos
    ];

    private static readonly PaginationQuery _defaultPaginationQuery =
        new() { Offset = 0, Limit = 5 };

    public CreateSosAlertHandlerTests() =>
        _sut = new CreateSosAlertHandler(
            _context,
            new FakeTimeProvider(_firedAt),
            _alertDispatcher
        );

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldCreateSosAlert()
    {
        var account = await _context.SetUpAccount();
        var seniorProfile = await _context.SetUpSeniorProfile(account);

        var request = new CreateSosAlertRequest { AccountId = account.Id, };

        await _sut.Handle(request, CancellationToken.None);

        await _alertDispatcher
            .Received()
            .Dispatch(
                Arg.Is<Alert>(
                    a =>
                        a.SeniorId == seniorProfile.SeniorId
                        && a.Type == AlertType.sos
                        && a.FiredAt == _firedAt
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_ShouldThrowSeniorNotFoundException_WhenSeniorProfileDoesNotExist()
    {
        var account = await _context.SetUpAccount();

        var request = new CreateSosAlertRequest { AccountId = account.Id, };

        var act = async () => await _sut.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<SeniorNotFoundException>();
    }
}
