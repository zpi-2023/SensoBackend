using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.PushNotifications.Payloads;

public sealed class MedicationToTakePayload : IPayload
{
    public async Task<(string title, string body, string priority)> Create(
        Alert alert,
        int receiverAccountId,
        AppDbContext context
    )
    {
        throw new NotImplementedException();
    }
}
