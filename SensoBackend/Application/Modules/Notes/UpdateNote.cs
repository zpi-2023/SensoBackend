using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using SensoBackend.Application.Modules.Notes.Contracts;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Notes;

public sealed record UpdateNoteRequest : IRequest<NoteDto>
{
    public required int AccountId { get; init; }
    public required int NoteId { get; init; }
    public required UpsertNoteDto Dto { get; init; }
}

[UsedImplicitly]
public sealed class UpdateNoteValidator : AbstractValidator<UpdateNoteRequest>
{
    public UpdateNoteValidator()
    {
        RuleFor(r => r.Dto.Content).NotEmpty().WithMessage("Content is required.");
        RuleFor(r => r.Dto.Title).MaximumLength(255).WithMessage("Title is too long.");
    }
}

[UsedImplicitly]
public sealed class UpdateNoteHandler : IRequestHandler<UpdateNoteRequest, NoteDto>
{
    private readonly AppDbContext _context;

    public UpdateNoteHandler(AppDbContext context) => _context = context;

    public async Task<NoteDto> Handle(UpdateNoteRequest request, CancellationToken ct)
    {
        var note =
            await _context.Notes.FindAsync(request.NoteId, ct)
            ?? throw new NoteNotFoundException(request.NoteId);

        if (note.AccountId != request.AccountId)
        {
            throw new NoteAccessDeniedException(note.Id);
        }

        note.Content = request.Dto.Content;
        note.Title = request.Dto.Title;
        note.IsPrivate = request.Dto.IsPrivate;
        await _context.SaveChangesAsync(ct);

        return note.Adapt<NoteDto>();
    }
}
