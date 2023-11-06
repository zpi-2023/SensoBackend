using JetBrains.Annotations;
using MediatR;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Notes;

public sealed record DeleteNoteRequest : IRequest
{
    public required int AccountId { get; init; }
    public required int NoteId { get; init; }
}

[UsedImplicitly]
public sealed class DeleteNoteHandler : IRequestHandler<DeleteNoteRequest>
{
    private readonly AppDbContext _context;

    public DeleteNoteHandler(AppDbContext context) => _context = context;

    public async Task Handle(DeleteNoteRequest request, CancellationToken ct)
    {
        var note =
            await _context.Notes.FindAsync(request.NoteId, ct)
            ?? throw new NoteNotFoundException(request.NoteId);

        if (note.AccountId != request.AccountId)
        {
            throw new NoteAccessDeniedException(note.Id);
        }

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync(ct);
    }
}
