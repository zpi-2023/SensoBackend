namespace SensoBackend.Domain.Entities;

public sealed class DashboardItem
{
    public required int Id { get; init; }

    public required int GadgetId { get; init; }

    public required int AccountId { get; init; }

    public required int Position { get; init; }

    public Gadget? Gadget { get; init; }

    public Account? Account { get; init; }
}
