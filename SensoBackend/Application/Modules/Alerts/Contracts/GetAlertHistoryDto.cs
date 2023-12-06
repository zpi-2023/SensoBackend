using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Alerts.Contracts;

public sealed record GetAlertHistoryDto
{
    [Required]
    public required string Type { get; init; }

    [Required]
    public required DateTimeOffset FiredAt { get; init; }
}
