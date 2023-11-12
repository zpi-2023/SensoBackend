namespace SensoBackend.Domain.Entities;

public class Reminder
{
    public required int Id { get; set; }

    public required int SeniorId { get; set; }

    public required int MedicationId { get; set; }

    public required bool IsActive { get; set; }

    public required float AmountPerIntake { get; set; }

    public float? AmountOwned { get; set; }

    public string? Cron { get; set; }

    public string? Description { get; set; }

    public Account? Senior { get; set; }

    public Medication? Medication { get; set; }
}
