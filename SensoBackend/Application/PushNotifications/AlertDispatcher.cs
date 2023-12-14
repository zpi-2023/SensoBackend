using Expo.Server.Client;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Abstractions;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.PushNotifications;

public sealed class AlertDispatcher(
    AppDbContext context,
    IPayloadFactory payloadFactory,
    ILogger<AlertDispatcher> logger
) : IAlertDispatcher
{
    private static readonly AlertType[] alertsSendToSenior = [AlertType.medicationToTake];
    private static readonly AlertType[] alertsSendToCaretaker =
    [
        AlertType.medicationNotTaken,
        AlertType.sos
    ];

    public async Task Dispatch(Alert alert, CancellationToken ct)
    {
        logger.LogInformation(
            "Dispatching alert {AlertId} of type {AlertType} for senior {SeniorId}",
            alert.Id,
            alert.Type,
            alert.SeniorId
        );

        await SaveAlertInHistory(alert, ct);
        await SendPushNotification(alert, ct);
    }

    private async Task SaveAlertInHistory(Alert alert, CancellationToken ct)
    {
        logger.LogInformation(
            "Saving alert {AlertId} of type {AlertType} for senior {SeniorId} in history",
            alert.Id,
            alert.Type,
            alert.SeniorId
        );

        await context.Alerts.AddAsync(alert, ct);
        await context.SaveChangesAsync(ct);

        logger.LogInformation(
            "Alert {AlertId} of type {AlertType} for senior {SeniorId} saved in history",
            alert.Id,
            alert.Type,
            alert.SeniorId
        );
    }

    private async Task SendPushNotification(Alert alert, CancellationToken ct)
    {
        logger.LogInformation(
            "Sending push notification for alert {AlertId} of type {AlertType} for senior {SeniorId}",
            alert.Id,
            alert.Type,
            alert.SeniorId
        );

        var devices = new List<Device>();

        if (alertsSendToCaretaker.Contains(alert.Type))
        {
            logger.LogInformation(
                "Gathering devices for alert {AlertId} of type {AlertType} for senior {SeniorId} to caretakers",
                alert.Id,
                alert.Type,
                alert.SeniorId
            );
            devices.AddRange(await GetCaretakersDevices(alert.SeniorId, ct));
        }

        if (alertsSendToSenior.Contains(alert.Type))
        {
            logger.LogInformation(
                "Gathering devices for alert {AlertId} of type {AlertType} for senior {SeniorId} to senior",
                alert.Id,
                alert.Type,
                alert.SeniorId
            );
            var device = await GetSeniorDevice(alert.SeniorId, ct);
            if (device is not null)
            {
                devices.Add(device);
            }
        }

        logger.LogInformation(
            "Number of devices to send push notification for alert {AlertId} of type {AlertType} for senior {SeniorId}: {DevicesCount}",
            alert.Id,
            alert.Type,
            alert.SeniorId,
            devices.Count
        );

        var expoPushClient = new PushApiClient();

        foreach (var device in devices)
        {
            logger.LogInformation(
                "Sending push notification for alert {AlertId} of type {AlertType} for senior {SeniorId} to device {DeviceId} with token {DeviceToken}",
                alert.Id,
                alert.Type,
                alert.SeniorId,
                device.Id,
                device.Token
            );

            var pushTicketRequest = await payloadFactory.Create(
                alert,
                device.AccountId,
                device.Token
            );
            var result = await expoPushClient.PushSendAsync(pushTicketRequest);

            logger.LogInformation(
                "Push notification for alert {AlertId} of type {AlertType} for senior {SeniorId} to device {DeviceId} with token {DeviceToken} sent with status {PushTicketStatus}",
                alert.Id,
                alert.Type,
                alert.SeniorId,
                device.Id,
                device.Token,
                result?.PushTicketStatuses[0]
            );

            if (result?.PushTicketErrors?.Count > 0)
            {
                foreach (var error in result.PushTicketErrors)
                {
                    logger.LogError(
                        "Error sending push notification: {ErrorCode} - {ErrorMessage}",
                        error.ErrorCode,
                        error.ErrorMessage
                    );
                }
            }
        }
    }

    private async Task<List<Device>> GetCaretakersDevices(int seniorId, CancellationToken ct)
    {
        var profiles = await context
            .Profiles
            .Where(p => p.SeniorId == seniorId && p.AccountId != seniorId)
            .ToListAsync(ct);

        var devices = new List<Device>();

        foreach (var profile in profiles)
        {
            var device = await context
                .Devices
                .Where(d => d.AccountId == profile.AccountId)
                .FirstOrDefaultAsync(ct);

            if (device is null)
            {
                continue;
            }

            if (device.Type == DeviceType.Ios)
            {
                // Not supported
                continue;
            }

            devices.Add(device);
        }

        return devices;
    }

    private async Task<Device?> GetSeniorDevice(int seniorId, CancellationToken ct)
    {
        var device = await context
            .Devices
            .Where(d => d.AccountId == seniorId)
            .FirstOrDefaultAsync(ct);

        if (device is null)
        {
            return null;
        }

        if (device.Type == DeviceType.Ios)
        {
            // Not supported
            return null;
        }

        return device;
    }
}
