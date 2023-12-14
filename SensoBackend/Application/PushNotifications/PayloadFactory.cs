using Expo.Server.Models;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.PushNotifications.Payloads;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.PushNotifications;

public sealed class PayloadFactory(AppDbContext context) : IPayloadFactory
{
    public async Task<PushTicketRequest> Create(Alert alert, int receiverAccountId, string token)
    {
        var payloadCreator = GetPayloadCreator(alert.Type);
        var (title, body, priority) = await payloadCreator.Create(
            alert,
            receiverAccountId,
            context
        );

        var pushTicketRequest = new PushTicketRequest
        {
            PushTo =  [token],
            PushTitle = title,
            PushBody = body,
            PushPriority = priority,
            PushData = new Dictionary<string, object>
            {
                { "alertId", alert.Id },
                { "alertType", alert.Type },
                { "seniorId", alert.SeniorId }
            },
        };

        return pushTicketRequest;
    }

    private static IPayload GetPayloadCreator(AlertType type)
    {
        return type switch
        {
            AlertType.sos => new SosPayload(),
            AlertType.medicationToTake => new MedicationToTakePayload(),
            AlertType.medicationNotTaken => new MedicationNotTakenPayload(),
            _ => throw new NotImplementedException(),
        };
    }
}
