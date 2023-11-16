using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Medications.Contracts;

public sealed record MedicationDto
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public float? AmountInPackage { get; init; }

    [Required]
    public string? AmountUnit { get; init; }
}
