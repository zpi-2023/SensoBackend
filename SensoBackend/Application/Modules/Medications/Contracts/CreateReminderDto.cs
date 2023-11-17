using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Medications.Contracts;

public sealed record CreateReminderDto
{
    [Required]
    public required string MedicationName { get; init; }

    public float? MedicationAmountInPackage { get; init; }

    [Required]
    public required float AmountPerIntake { get; init; }

    public float? AmountOwned { get; init; }

    public string? AmountUnit { get; init; }

    public string? Cron { get; init; }

    public string? Description { get; init; }
}
