using System.ComponentModel.DataAnnotations;

namespace SensoBackend.Application.Modules.Notes.Contracts;

public sealed record NoteListDto
{
    [Required]
    public required List<NoteDto> Notes { get; init; }
}
