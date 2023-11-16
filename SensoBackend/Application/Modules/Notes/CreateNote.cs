using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Notes.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Notes;

public sealed record CreateNoteRequest : IRequest<NoteDto>
{
    public required int AccountId { get; init; }
    public required UpsertNoteDto Dto { get; init; }
}

[UsedImplicitly]
public sealed class CreateNoteValidator : AbstractValidator<CreateNoteRequest>
{
    public CreateNoteValidator()
    {
        RuleFor(r => r.Dto.Content).NotEmpty().WithMessage("Content is required.");
        RuleFor(r => r.Dto.Title).MaximumLength(255).WithMessage("Title is too long.");
    }
}

[UsedImplicitly]
public sealed class CreateNoteHandler : IRequestHandler<CreateNoteRequest, NoteDto>
{
    private readonly AppDbContext _context;
    private readonly ITimeProvider _timeProvider;

    public CreateNoteHandler(AppDbContext context, ITimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async Task<NoteDto> Handle(CreateNoteRequest request, CancellationToken ct)
    {
        await GuardHasSeniorProfileAsync(request.AccountId, ct);

        var note = new Note
        {
            Id = default,
            AccountId = request.AccountId,
            Content = request.Dto.Content,
            CreatedAt = _timeProvider.Now,
            IsPrivate = request.Dto.IsPrivate,
            Title = request.Dto.Title,
        };

        await _context.Notes.AddAsync(note, ct);
        await _context.SaveChangesAsync(ct);

        return note.Adapt<NoteDto>();
    }

    private async Task GuardHasSeniorProfileAsync(int accountId, CancellationToken ct)
    {
        if (
            !await _context.Profiles.AnyAsync(
                p => p.AccountId == accountId && p.AccountId == p.SeniorId,
                ct
            )
        )
        {
            throw new SeniorNotFoundException("Given account does not have a senior profile");
        }
    }
}
