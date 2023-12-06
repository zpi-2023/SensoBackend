using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Alerts.Contracts;

public sealed record CreateAlertDto
{
    [Required]
    public required string Type { get; init; }
}
