using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.Utils;
using SensoBackend.Domain.Entities;
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

        var deviceInDb = await context
            .Devices
            .Where(d => d.AccountId == request.AccountId && d.Type == deviceType)
            .FirstOrDefaultAsync(ct);

        if (deviceInDb is null)
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
            return;
        }

        if (deviceInDb.Token == request.Dto.DeviceToken)
        {
            return;
        }

        deviceInDb.Token = request.Dto.DeviceToken;
        deviceInDb.AddedAt = timeProvider.GetUtcNow();
        await context.SaveChangesAsync(ct);
    }
}
