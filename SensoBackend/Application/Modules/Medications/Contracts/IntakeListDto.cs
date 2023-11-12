using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Medications.Contracts;

public sealed record IntakeListDto
{
    [Required]
    public required List<IntakeDto> Intakes { get; init; }
}
