using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Notes.Contracts;

public sealed record UpsertNoteDto
{
    [Required]
    public required string Content { get; init; }

    [Required]
    public required bool IsPrivate { get; init; }

    [Required]
    public string? Title { get; init; }
}
