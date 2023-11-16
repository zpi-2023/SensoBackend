using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Medications.Contracts;

public sealed record CreateIntakeDto
{
    [Required]
    public required DateTimeOffset TakenAt { get; init; }

    [Required]
    public required float AmountTaken { get; init; }
}
