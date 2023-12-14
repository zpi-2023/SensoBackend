using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.PushNotifications.Payloads;

public sealed class MedicationToTakePayload : IPayload
{
    public Task<(string title, string body, string priority)> Create(
        Alert alert,
        int receiverAccountId,
        AppDbContext context
    )
    {
        var title = "Senso - Medication to take!";
        var body = $"You need to take your medication!";
        var priority = "default";

        return Task.FromResult((title, body, priority));
    }
}
