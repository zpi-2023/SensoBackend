using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.PushNotifications.Payloads;

public interface IPayload
{
    Task<(string title, string body, string priority)> Create(
        Alert alert,
        int receiverAccountId,
        AppDbContext context
    );
}
