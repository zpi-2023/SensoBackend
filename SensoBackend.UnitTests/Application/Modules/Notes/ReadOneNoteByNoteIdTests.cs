using SensoBackend.Application.Modules.Notes;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Notes;

public sealed class ReadOneNoteByNoteIdHandlerTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly ReadOneNoteByNoteIdHandler _sut;

    public ReadOneNoteByNoteIdHandlerTests() => _sut = new ReadOneNoteByNoteIdHandler(_context);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_ShouldReturnNote_WhenSeniorRequests(bool isPrivate)
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);
        var note = await _context.SetUpNote(seniorAccount, isPrivate);

        var noteDto = await _sut.Handle(
            new ReadOneNoteByNoteIdRequest { AccountId = seniorAccount.Id, NoteId = note.Id },
            CancellationToken.None
        );

        noteDto.Id.Should().Be(note.Id);
        noteDto.Content.Should().Be(note.Content);
        noteDto.CreatedAt.Should().Be(note.CreatedAt);
        noteDto.IsPrivate.Should().Be(note.IsPrivate);
        noteDto.Title.Should().Be(note.Title);
    }

    [Fact]
    public async Task Handle_ShouldThrowNoteNotFoundException_WhenNoteIdIsInvalid()
    {
        var seniorAccount = await _context.SetUpAccount();

        var action = async () =>
            await _sut.Handle(
                new ReadOneNoteByNoteIdRequest { AccountId = seniorAccount.Id, NoteId = 32 },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<NoteNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldReturnNote_WhenCaretakerRequestsPublicNote()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);
        var note = await _context.SetUpNote(seniorAccount, isPrivate: false);
        var caretakerAccount = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretakerAccount, seniorAccount);

        var noteDto = await _sut.Handle(
            new ReadOneNoteByNoteIdRequest { AccountId = caretakerAccount.Id, NoteId = note.Id },
            CancellationToken.None
        );

        noteDto.Id.Should().Be(note.Id);
        noteDto.Content.Should().Be(note.Content);
        noteDto.CreatedAt.Should().Be(note.CreatedAt);
        noteDto.IsPrivate.Should().Be(note.IsPrivate);
        noteDto.Title.Should().Be(note.Title);
    }

    [Fact]
    public async Task Handle_ShouldThrowNoteAccessDeniedException_WhenCaretakerRequestsPrivateNote()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);
        var note = await _context.SetUpNote(seniorAccount, isPrivate: true);
        var caretakerAccount = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretakerAccount, seniorAccount);

        var action = async () =>
            await _sut.Handle(
                new ReadOneNoteByNoteIdRequest
                {
                    AccountId = caretakerAccount.Id,
                    NoteId = note.Id
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<NoteAccessDeniedException>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_ShouldThrowNoteAccessDeniedException_WhenRandomAccountRequests(
        bool isPrivate
    )
    {
        var account = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(account);
        var note = await _context.SetUpNote(account, isPrivate);
        var impostor = await _context.SetUpAccount();

        var action = async () =>
            await _sut.Handle(
                new ReadOneNoteByNoteIdRequest { AccountId = impostor.Id, NoteId = note.Id },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<NoteAccessDeniedException>();
    }
}
