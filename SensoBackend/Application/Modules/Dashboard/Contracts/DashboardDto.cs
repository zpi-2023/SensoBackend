using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Dashboard.Contracts;

public sealed record DashboardDto
{
    [Required]
    public required List<string> Gadgets { get; init; }
}
