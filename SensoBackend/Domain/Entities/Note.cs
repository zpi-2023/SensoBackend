namespace SensoBackend.Domain.Entities;

public sealed class Note
{
    public required int Id { get; init; }

    public required int AccountId { get; init; }

    public required string Content { get; set; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required bool IsPrivate { get; set; }

    public string? Title { get; set; }

    public Account? Account { get; init; }
}
