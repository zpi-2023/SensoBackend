using System.Linq.Expressions;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Notes.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Notes;

public sealed record ReadAllNotesBySeniorIdRequest : IRequest<NoteListDto>
{
    public required int AccountId { get; init; }
    public required int SeniorId { get; init; }
}

[UsedImplicitly]
public sealed class ReadAllNotesBySeniorIdHandler(AppDbContext context)
    : IRequestHandler<ReadAllNotesBySeniorIdRequest, NoteListDto>
{
    public async Task<NoteListDto> Handle(
        ReadAllNotesBySeniorIdRequest request,
        CancellationToken ct
    )
    {
        var notes = await context
            .Notes
            .Where(IsNoteVisible(request))
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

        return new NoteListDto { Notes = notes.Adapt<List<NoteDto>>() };
    }

    private static Expression<Func<Note, bool>> IsNoteVisible(ReadAllNotesBySeniorIdRequest request)
    {
        if (request.AccountId == request.SeniorId) // The senior is making the request
        {
            return note => note.AccountId == request.SeniorId;
        }
        else // The caretaker is making the request
        {
            return note => note.AccountId == request.SeniorId && !note.IsPrivate;
        }
    }
}
