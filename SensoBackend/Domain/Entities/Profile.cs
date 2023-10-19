namespace SensoBackend.Domain.Entities;

public sealed class Profile
{
    public required int Id { get; set; }
    public required int AccountId { get; set; }
    public required int SeniorId { get; set; }
    public string? Alias { get; set; }

    public Account? Account { get; set; }
    public Account? Senior { get; set; }
}
