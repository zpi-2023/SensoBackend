using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Notes.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Notes;

public sealed record ReadOneNoteByNoteIdRequest : IRequest<NoteDto>
{
    public required int AccountId { get; init; }
    public required int NoteId { get; init; }
}

[UsedImplicitly]
public sealed class ReadOneNoteByNoteIdHandler
    : IRequestHandler<ReadOneNoteByNoteIdRequest, NoteDto>
{
    private readonly AppDbContext _context;

    public ReadOneNoteByNoteIdHandler(AppDbContext context) => _context = context;

    public async Task<NoteDto> Handle(ReadOneNoteByNoteIdRequest request, CancellationToken ct)
    {
        var note =
            await _context.Notes.FindAsync(request.NoteId, ct)
            ?? throw new NoteNotFoundException(request.NoteId);

        if (!await HasAccessAsync(note, request.AccountId, ct))
        {
            throw new NoteAccessDeniedException(note.Id);
        }

        return note.Adapt<NoteDto>();
    }

    private async Task<bool> HasAccessAsync(Note note, int accountId, CancellationToken ct)
    {
        if (note.AccountId == accountId) // The user is the owner of the note
        {
            return true;
        }

        if (
            !note.IsPrivate
            && await _context.Profiles.AnyAsync(
                p => p.SeniorId == note.AccountId && p.AccountId == accountId,
                ct
            )
        ) // The user is a caretaker of the senior, and the note is not private
        {
            return true;
        }

        return false;
    }
}
