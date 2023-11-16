using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Medications.Contracts;

public sealed record MedicationListDto
{
    [Required]
    public required List<MedicationDto> Medications { get; init; }
}
