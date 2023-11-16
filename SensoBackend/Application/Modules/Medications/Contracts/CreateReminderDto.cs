using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Medications.Contracts;

public sealed record CreateReminderDto
{
    [Required]
    public required string MeicationName { get; init; }

    public float? MedicationAmountInPackage { get; init; }

    [Required]
    public required float AmountPerIntake { get; init; }

    [Required]
    public float? AmountOwned { get; init; }

    [Required]
    public string? AmountUnit { get; init; }

    [Required]
    public string? Cron { get; init; }

    [Required]
    public string? Description { get; init; }
}
