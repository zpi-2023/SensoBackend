using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.Utils;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Accounts.CreateAccount;

public sealed record AddDeviceTokenRequest : IRequest
{
    public required int AccountId;
    public required AddDeviceTokenDto Dto;
}

[UsedImplicitly]
public sealed class AddDeviceTokenValidator : AbstractValidator<AddDeviceTokenRequest>
{
    public AddDeviceTokenValidator()
    {
        RuleFor(r => r.Dto.DeviceToken).NotEmpty().WithMessage("Device token is required");
        RuleFor(r => r.Dto.DeviceType).NotEmpty().WithMessage("Device type is required");
    }
}

[UsedImplicitly]
public sealed class AddDeviceTokenHandler(AppDbContext context, TimeProvider timeProvider)
    : IRequestHandler<AddDeviceTokenRequest>
{
    public async Task Handle(AddDeviceTokenRequest request, CancellationToken ct)
    {
        var deviceType = GetDeviceType.FromName(request.Dto.DeviceType);

        if (await IsDeviceTokenAssignedToAccount(request, deviceType, ct))
        {
            return;
        }

        if (await IsDeviceTokenAssignedToAnyAccount(request, deviceType, ct))
        {
            await RemoveDeviceTokenFromAllAccounts(request, deviceType, ct);
        }

        if (await IsAnyDeviceTokenAssignedToAccount(request, ct))
        {
            await RemoveAccountDeviceTokens(request, ct);
        }

        await CreateDeviceTokenForAccount(request, deviceType, ct);
    }

    private async Task<bool> IsDeviceTokenAssignedToAccount(
        AddDeviceTokenRequest request,
        DeviceType deviceType,
        CancellationToken ct
    )
    {
        var device = await context
            .Devices
            .FirstOrDefaultAsync(
                d =>
                    d.AccountId == request.AccountId
                    && d.Token == request.Dto.DeviceToken
                    && d.Type == deviceType,
                ct
            );

        return device is not null;
    }

    private async Task<bool> IsDeviceTokenAssignedToAnyAccount(
        AddDeviceTokenRequest request,
        DeviceType deviceType,
        CancellationToken ct = default
    )
    {
        var device = await context
            .Devices
            .FirstOrDefaultAsync(
                d => d.Token == request.Dto.DeviceToken && d.Type == deviceType,
                ct
            );

        return device is not null;
    }

    private async Task<bool> IsAnyDeviceTokenAssignedToAccount(
        AddDeviceTokenRequest request,
        CancellationToken ct = default
    )
    {
        var device = await context
            .Devices
            .FirstOrDefaultAsync(d => d.AccountId == request.AccountId, ct);

        return device is not null;
    }

    private async Task RemoveDeviceTokenFromAllAccounts(
        AddDeviceTokenRequest request,
        DeviceType deviceType,
        CancellationToken ct = default
    )
    {
        var devices = await context
            .Devices
            .Where(d => d.Token == request.Dto.DeviceToken && d.Type == deviceType)
            .ToListAsync(ct);

        context.Devices.RemoveRange(devices);
        await context.SaveChangesAsync(ct);
    }

    private async Task RemoveAccountDeviceTokens(
        AddDeviceTokenRequest request,
        CancellationToken ct = default
    )
    {
        var devices = await context
            .Devices
            .Where(d => d.AccountId == request.AccountId)
            .ToListAsync(ct);

        context.Devices.RemoveRange(devices);
        await context.SaveChangesAsync(ct);
    }

    private async Task CreateDeviceTokenForAccount(
        AddDeviceTokenRequest request,
        DeviceType deviceType,
        CancellationToken ct = default
    )
    {
        var device = new Device
        {
            Id = default,
            AccountId = request.AccountId,
            Token = request.Dto.DeviceToken,
            Type = deviceType,
            AddedAt = timeProvider.GetUtcNow(),
        };

        context.Devices.Add(device);
        await context.SaveChangesAsync(ct);
    }
}
