using Expo.Server.Client;
using Expo.Server.Models;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Alerts;

public sealed record DispatchAlertRequest : IRequest
{
    public required Alert Alert { get; init; }
}

[UsedImplicitly]
public sealed class DispatchAlertHandler(AppDbContext context, ILogger<DispatchAlertHandler> logger)
    : IRequestHandler<DispatchAlertRequest>
{
    private static readonly AlertType[] alertToSenior = [AlertType.medicationToTake];
    private static readonly AlertType[] alertToCaretaker =
    [
        AlertType.medicationNotTaken,
        AlertType.sos
    ];

    public async Task Handle(DispatchAlertRequest request, CancellationToken ct)
    {
        await context.Alerts.AddAsync(request.Alert, ct);
        await context.SaveChangesAsync(ct);

        if (alertToSenior.Contains(request.Alert.Type))
        {
            _ = SendAlertToSenior(request.Alert, ct);
        }

        if (alertToCaretaker.Contains(request.Alert.Type))
        {
            _ = SendAlertToCaretaker(request.Alert, ct);
        }
    }

    private async Task SendAlertToSenior(Alert alert, CancellationToken ct)
    {
        var device = await context
            .Devices
            .Where(d => d.AccountId == alert.SeniorId)
            .FirstOrDefaultAsync(ct);

        if (device is null)
        {
            return;
        }

        if (device.Type == DeviceType.Ios)
        {
            // Not supported yet
            return;
        }

        var tokens = new List<string> { device.Token };
        await SendAlertToExpo(tokens, alert, ct);
    }

    private async Task SendAlertToCaretaker(Alert alert, CancellationToken ct)
    {
        var caretakerProfiles = await context
            .Profiles
            .Where(p => p.SeniorId == alert.SeniorId)
            .ToListAsync(ct);

        var tokens = new List<string>();
        foreach (var caretakerProfile in caretakerProfiles)
        {
            var device = await context
                .Devices
                .Where(d => d.AccountId == caretakerProfile.AccountId)
                .FirstOrDefaultAsync(ct);

            if (device is null)
            {
                continue;
            }

            if (device.Type == DeviceType.Ios)
            {
                // Not supported yet
                continue;
            }

            tokens.Add(device.Token);
        }
        await SendAlertToExpo(tokens, alert, ct);
    }

    private static string CreateAlertMessage(Alert alert) =>
        alert.Type switch
        {
            AlertType.medicationToTake => "It's time to take your medication!",
            AlertType.medicationNotTaken => "Your senior didn't take their medication!",
            AlertType.sos => "SOS! Your senior needs help!",
            _ => "",
        };

    private async Task SendAlertToExpo(List<string> tokens, Alert alert, CancellationToken ct)
    {
        var expoPushClient = new PushApiClient();
        var pushTicketRequest = new PushTicketRequest
        {
            PushTo = tokens,
            PushBadgeCount = 1,
            PushSound = "default",
            PushTitle = "Senso",
            PushBody = CreateAlertMessage(alert),
            PushData = new Dictionary<string, object>
            {
                { "alertType", alert.Type },
                { "seniorId", alert.SeniorId }
            }
        };

        var result = await expoPushClient.PushSendAsync(pushTicketRequest);

        if (result?.PushTicketErrors?.Count > 0)
        {
            foreach (var error in result.PushTicketErrors)
            {
                logger.LogError(
                    "Error sending push notification: {error.ErrorCode} - {error.ErrorMessage}",
                    error.ErrorCode,
                    error.ErrorMessage
                );
            }
        }
    }
}
