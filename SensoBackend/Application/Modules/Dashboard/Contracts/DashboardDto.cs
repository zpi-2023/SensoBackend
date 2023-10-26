namespace SensoBackend.Application.Modules.Dashboard.Contracts;

public sealed record DashboardDto
{
    public required List<string> Gadgets { get; init; }
}
