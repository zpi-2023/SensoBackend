using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Domain.Entities;

public class IntakeRecord
{
    public required int Id { get; set; }

    public required int ReminderId { get; set; }

    [DataType(DataType.DateTime)]
    public required DateTimeOffset TakenAt { get; set; }

    public required float AmountTaken { get; set; }

    public Reminder? Reminder { get; set; }
}
