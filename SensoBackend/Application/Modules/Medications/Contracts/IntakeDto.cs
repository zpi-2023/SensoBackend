using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Medications.Contracts;

public sealed record IntakeDto
{
    [Required]
    public required int Id { get; init; }

    [Required]
    public required int ReminderId { get; init; }

    [Required]
    public required string MedicationName { get; init; }

    [Required]
    public required DateTimeOffset TakenAt { get; init; }

    [Required]
    public required float AmountTaken { get; init; }

    public string? AmountUnit { get; init; }
}
