namespace SensoBackend.Domain.Entities;

public class Medication
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public float? AmountInPackage { get; set; }

    public string? AmountUnit { get; set; }
}
