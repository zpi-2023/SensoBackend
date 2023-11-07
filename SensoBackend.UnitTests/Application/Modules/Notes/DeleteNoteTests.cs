using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Notes;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Notes;

public sealed class DeleteNoteHandlerTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly DeleteNoteHandler _sut;

    public DeleteNoteHandlerTests() => _sut = new DeleteNoteHandler(_context);

    [Fact]
    public async Task Handle_ShouldDeleteNote_WhenAllConditionsAreSatisfied()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);
        var note = await _context.SetUpNote(seniorAccount);

        await _sut.Handle(
            new DeleteNoteRequest { AccountId = seniorAccount.Id, NoteId = note.Id },
            CancellationToken.None
        );

        var noteExists = await _context.Notes.AnyAsync(n => n.Id == note.Id);
        noteExists.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldThrowNoteNotFoundException_WhenTheNoteIdIsInvalid()
    {
        var action = async () =>
            await _sut.Handle(
                new DeleteNoteRequest { AccountId = 15, NoteId = 32 },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<NoteNotFoundException>();
    }

    [Fact]
    public async Task Handle_ThrowNoteAccessDeniedException_WhenAnotherUserTriesToDelete()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);
        var note = await _context.SetUpNote(seniorAccount);
        var impostor = await _context.SetUpAccount();

        var action = async () =>
            await _sut.Handle(
                new DeleteNoteRequest { AccountId = impostor.Id, NoteId = note.Id },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<NoteAccessDeniedException>();
    }
}
