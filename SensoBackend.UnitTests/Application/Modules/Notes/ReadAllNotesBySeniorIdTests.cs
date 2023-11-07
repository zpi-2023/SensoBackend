using SensoBackend.Application.Modules.Notes;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Notes;

public sealed class ReadAllNotesBySeniorIdHandlerTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly ReadAllNotesBySeniorIdHandler _sut;

    public ReadAllNotesBySeniorIdHandlerTests() =>
        _sut = new ReadAllNotesBySeniorIdHandler(_context);

    private async Task<(Account seniorAccount, Note[] notes)> SetUpMockNotes()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);
        var notes = new Note[]
        {
            await _context.SetUpNote(seniorAccount, isPrivate: true),
            await _context.SetUpNote(seniorAccount, isPrivate: true),
            await _context.SetUpNote(seniorAccount, isPrivate: true),
            await _context.SetUpNote(seniorAccount, isPrivate: false),
            await _context.SetUpNote(seniorAccount, isPrivate: false),
        };
        return (seniorAccount, notes);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllNotes_WhenSeniorRequests()
    {
        var (seniorAccount, notes) = await SetUpMockNotes();

        var noteListDto = await _sut.Handle(
            new ReadAllNotesBySeniorIdRequest
            {
                AccountId = seniorAccount.Id,
                SeniorId = seniorAccount.Id
            },
            CancellationToken.None
        );

        noteListDto.Notes.Should().HaveCount(notes.Length);
    }

    [Fact]
    public async Task Handle_ShouldOnlyReturnPublicNotes_WhenCaretakerRequests()
    {
        var (seniorAccount, notes) = await SetUpMockNotes();
        var caretakerAccount = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretakerAccount, seniorAccount);

        var noteListDto = await _sut.Handle(
            new ReadAllNotesBySeniorIdRequest
            {
                AccountId = caretakerAccount.Id,
                SeniorId = seniorAccount.Id
            },
            CancellationToken.None
        );

        noteListDto.Notes.Should().HaveCount(notes.Where(n => !n.IsPrivate).Count());
    }
}
