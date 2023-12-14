using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.PushNotifications.Payloads;

public sealed class MedicationNotTakenPayload : IPayload
{
    public async Task<(string title, string body, string priority)> Create(
        Alert alert,
        int receiverAccountId,
        AppDbContext context
    )
    {
        var profile =
            await context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.SeniorId == alert.SeniorId && p.AccountId == receiverAccountId
                ) ?? throw new InvalidOperationException();

        var title = "Senso - Medication not taken!";
        var body = $"Your senior - {profile.Alias} - does not take his medication!";
        var priority = "default";

        return (title, body, priority);
    }
}
