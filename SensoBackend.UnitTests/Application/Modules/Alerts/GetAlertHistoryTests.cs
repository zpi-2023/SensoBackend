using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Alerts;
using SensoBackend.Application.Modules.Alerts.Contracts;
using SensoBackend.Domain.Enums;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Alerts;

public sealed class GetAlertHistoryHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetAlertHistoryHandler _sut;

    private static readonly string _validAlertTypeName = "sos";
    private static readonly AlertType _alertType = AlertType.sos;

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

    public GetAlertHistoryHandlerTests() => _sut = new GetAlertHistoryHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnAlertHistory_AsSenior()
    {
        var expectedAlertHistory = new List<GetAlertHistoryDto>();

        var account = await _context.SetUpAccount();
        var seniorProfile = await _context.SetUpSeniorProfile(account);

        for (int i = 0; i < _defaultPaginationQuery.Limit; i++)
        {
            var alert = await _context.SetUpAlert(seniorProfile, _alertTypes[i]);
            expectedAlertHistory.Add(
                new GetAlertHistoryDto { Type = alert.Type.ToString(), FiredAt = alert.FiredAt, }
            );
        }

        var request = new GetAlertHistoryRequest
        {
            AccountId = account.Id,
            SeniorId = seniorProfile.SeniorId,
            PaginationQuery = _defaultPaginationQuery,
        };

        var result = await _sut.Handle(request, CancellationToken.None);

        result.Should().BeOfType<PaginatedDto<GetAlertHistoryDto>>();
        result.Items.Should().BeEquivalentTo(expectedAlertHistory);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlertHistory_AsCaretaker()
    {
        var expectedAlertHistory = new List<GetAlertHistoryDto>();

        var seniorAccount = await _context.SetUpAccount();
        var seniorProfile = await _context.SetUpSeniorProfile(seniorAccount);

        var caretakerAccount = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretakerAccount, seniorAccount);

        for (int i = 0; i < _defaultPaginationQuery.Limit; i++)
        {
            var alert = await _context.SetUpAlert(seniorProfile, _alertTypes[i]);
            expectedAlertHistory.Add(
                new GetAlertHistoryDto { Type = alert.Type.ToString(), FiredAt = alert.FiredAt, }
            );
        }

        var request = new GetAlertHistoryRequest
        {
            AccountId = caretakerAccount.Id,
            SeniorId = seniorProfile.SeniorId,
            PaginationQuery = _defaultPaginationQuery,
        };

        var result = await _sut.Handle(request, CancellationToken.None);

        result.Should().BeOfType<PaginatedDto<GetAlertHistoryDto>>();
        result.Items.Should().BeEquivalentTo(expectedAlertHistory);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlertHistory_WithAlertTypeFilterAsSenior()
    {
        var expectedAlertHistory = new List<GetAlertHistoryDto>();

        var account = await _context.SetUpAccount();
        var seniorProfile = await _context.SetUpSeniorProfile(account);

        for (int i = 0; i < _defaultPaginationQuery.Limit; i++)
        {
            var alert = await _context.SetUpAlert(seniorProfile, _alertTypes[i]);
            if (alert.Type == _alertType)
                expectedAlertHistory.Add(
                    new GetAlertHistoryDto
                    {
                        Type = alert.Type.ToString(),
                        FiredAt = alert.FiredAt,
                    }
                );
        }

        var request = new GetAlertHistoryRequest
        {
            AccountId = account.Id,
            SeniorId = seniorProfile.SeniorId,
            PaginationQuery = _defaultPaginationQuery,
            Type = _validAlertTypeName
        };

        var result = await _sut.Handle(request, CancellationToken.None);

        result.Should().BeOfType<PaginatedDto<GetAlertHistoryDto>>();
        result.Items.Should().BeEquivalentTo(expectedAlertHistory);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlertHistory_WithAlertTypeFilterAsCaretaker()
    {
        var expectedAlertHistory = new List<GetAlertHistoryDto>();

        var seniorAccount = await _context.SetUpAccount();
        var seniorProfile = await _context.SetUpSeniorProfile(seniorAccount);

        var caretakerAccount = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretakerAccount, seniorAccount);

        for (int i = 0; i < _defaultPaginationQuery.Limit; i++)
        {
            var alert = await _context.SetUpAlert(seniorProfile, _alertTypes[i]);
            if (alert.Type == _alertType)
                expectedAlertHistory.Add(
                    new GetAlertHistoryDto
                    {
                        Type = alert.Type.ToString(),
                        FiredAt = alert.FiredAt,
                    }
                );
        }

        var request = new GetAlertHistoryRequest
        {
            AccountId = caretakerAccount.Id,
            SeniorId = seniorProfile.SeniorId,
            PaginationQuery = _defaultPaginationQuery,
            Type = _validAlertTypeName
        };

        var result = await _sut.Handle(request, CancellationToken.None);

        result.Should().BeOfType<PaginatedDto<GetAlertHistoryDto>>();
        result.Items.Should().BeEquivalentTo(expectedAlertHistory);
    }

    [Fact]
    public async Task Handle_ShouldThrowProfileNotFoundException_WhenSeniorProfileDoesNotExist()
    {
        var account = await _context.SetUpAccount();

        var request = new GetAlertHistoryRequest
        {
            AccountId = account.Id,
            SeniorId = account.Id,
            PaginationQuery = _defaultPaginationQuery,
        };

        var act = async () => await _sut.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<ProfileNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowProfileNotFoundException_WhenCaretakerProfileAssociatedWithSeniorDoesNotExist()
    {
        var seniorAccount = await _context.SetUpAccount();
        var seniorProfile = await _context.SetUpSeniorProfile(seniorAccount);
        var account = await _context.SetUpAccount();

        var request = new GetAlertHistoryRequest
        {
            AccountId = account.Id,
            SeniorId = seniorAccount.Id,
            PaginationQuery = _defaultPaginationQuery,
        };

        var act = async () => await _sut.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<ProfileNotFoundException>();
    }
}
