using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.CreateAccount;
using SensoBackend.Domain.Enums;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Accounts.CreateAccount;

public sealed class AddDeviceTokenHandlerTests : IDisposable
{
    private static readonly DateTimeOffset AddedAt = DateTimeOffset.UtcNow;

    private readonly AppDbContext _context = Database.CreateFixture();

    private static readonly string _deviceTypeName = "Android";
    private static readonly DeviceType _deviceType = DeviceType.Android;
    private static readonly string _deviceToken = "J3b4cDr4p4l3";

    private readonly AddDeviceTokenHandler _sut;

    public AddDeviceTokenHandlerTests() =>
        _sut = new AddDeviceTokenHandler(_context, new FakeTimeProvider(AddedAt));

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldAddDeviceToken()
    {
        var account = await _context.SetUpAccount();
        var dto = new AddDeviceTokenDto
        {
            DeviceToken = _deviceToken,
            DeviceType = _deviceTypeName,
        };

        var request = new AddDeviceTokenRequest { AccountId = account.Id, Dto = dto };
        await _sut.Handle(request, CancellationToken.None);

        var device = await _context.Devices.FirstOrDefaultAsync(d => d.AccountId == account.Id);

        device.Should().NotBeNull();
        if (device == null)
            return;

        device.Token.Should().Be(_deviceToken);
        device.Type.Should().Be(_deviceType);
        device.AddedAt.Should().Be(AddedAt);
    }

    [Fact]
    public async Task Handle_ShouldUpdateDeviceToken()
    {
        var account = await _context.SetUpAccount();
        await _context.SetUpDevice(account.Id, "oldToken", _deviceType);
        var dto = new AddDeviceTokenDto
        {
            DeviceToken = _deviceToken,
            DeviceType = _deviceTypeName,
        };

        var request = new AddDeviceTokenRequest { AccountId = account.Id, Dto = dto };
        await _sut.Handle(request, CancellationToken.None);

        var deviceInDb = await _context.Devices.FirstOrDefaultAsync(d => d.AccountId == account.Id);

        deviceInDb.Should().NotBeNull();
        if (deviceInDb == null)
            return;

        deviceInDb.Token.Should().Be(_deviceToken);
        deviceInDb.Type.Should().Be(_deviceType);
        deviceInDb.AddedAt.Should().Be(AddedAt);
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateDeviceToken_WhenDeviceTokenIsTheSame()
    {
        var account = await _context.SetUpAccount();
        var device = await _context.SetUpDevice(account.Id, _deviceToken, _deviceType);
        var dto = new AddDeviceTokenDto
        {
            DeviceToken = _deviceToken,
            DeviceType = _deviceTypeName,
        };

        var request = new AddDeviceTokenRequest { AccountId = account.Id, Dto = dto };
        await _sut.Handle(request, CancellationToken.None);

        var deviceInDb = await _context.Devices.FirstOrDefaultAsync(d => d.AccountId == account.Id);

        deviceInDb.Should().NotBeNull();
        if (deviceInDb == null)
            return;

        deviceInDb.Token.Should().Be(_deviceToken);
        deviceInDb.Type.Should().Be(_deviceType);
        deviceInDb.AddedAt.Should().Be(device.AddedAt);
    }

    [Fact]
    public async Task Handle_ShouldUpdateDeviceToken_WhenDeviceTokenIsAssignedToAnotherAccount()
    {
        var account = await _context.SetUpAccount();
        var anotherAccount = await _context.SetUpAccount();
        await _context.SetUpDevice(anotherAccount.Id, _deviceToken, _deviceType);
        var dto = new AddDeviceTokenDto
        {
            DeviceToken = _deviceToken,
            DeviceType = _deviceTypeName,
        };

        var request = new AddDeviceTokenRequest { AccountId = account.Id, Dto = dto };
        await _sut.Handle(request, CancellationToken.None);

        var anotherDeviceInDb = await _context
            .Devices
            .FirstOrDefaultAsync(d => d.AccountId == anotherAccount.Id);

        anotherDeviceInDb.Should().BeNull();

        var deviceInDb = await _context.Devices.FirstOrDefaultAsync(d => d.AccountId == account.Id);

        deviceInDb.Should().NotBeNull();
        if (deviceInDb == null)
            return;

        deviceInDb.Token.Should().Be(_deviceToken);
        deviceInDb.Type.Should().Be(_deviceType);
        deviceInDb.AddedAt.Should().Be(AddedAt);
    }
}
