using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Medications.Contracts;

public sealed record ReminderListDto
{
    [Required]
    public required List<ReminderDto> Reminders { get; init; }
}
