using SensoBackend.Domain.Enums;

namespace SensoBackend.Domain.Entities;

public sealed class DashboardItem
{
    public required int Id { get; init; }

    public required Gadget Gadget { get; init; }

    public required int AccountId { get; init; }

    public required int Position { get; init; }

    public Account? Account { get; init; }
}
