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
public sealed class DeleteNoteHandler(AppDbContext context) : IRequestHandler<DeleteNoteRequest>
{
    public async Task Handle(DeleteNoteRequest request, CancellationToken ct)
    {
        var note =
            await context.Notes.FindAsync(request.NoteId, ct)
            ?? throw new NoteNotFoundException(request.NoteId);

        if (note.AccountId != request.AccountId)
        {
            throw new NoteAccessDeniedException(note.Id);
        }

        context.Notes.Remove(note);
        await context.SaveChangesAsync(ct);
    }
}
