using Expo.Server.Models;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Application.Abstractions;

public interface IPayloadFactory
{
    Task<PushTicketRequest> Create(Alert alert, int receiverAccountId, string token);
}
