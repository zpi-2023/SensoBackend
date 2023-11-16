using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Notes.Contracts;

public sealed record NoteDto
{
    [Required]
    public required int Id { get; init; }

    [Required]
    public required string Content { get; init; }

    [Required]
    public required DateTimeOffset CreatedAt { get; init; }

    [Required]
    public required bool IsPrivate { get; init; }

    [Required]
    public string? Title { get; init; }
}
