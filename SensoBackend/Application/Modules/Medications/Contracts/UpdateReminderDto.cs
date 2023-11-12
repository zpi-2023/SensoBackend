using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Medications.Contracts;

public sealed record UpdateReminderDto
{
    [Required]
    public required float AmountPerIntake { get; init; }

    public float? AmountOwned { get; init; }

    public string? Cron { get; init; }
    
    public string? Description { get; init; }
}
