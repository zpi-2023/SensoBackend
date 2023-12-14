using SensoBackend.Domain.Entities;

namespace SensoBackend.Application.Abstractions;

public interface IAlertDispatcher
{
    Task Dispatch(Alert alert, CancellationToken ct);
}
