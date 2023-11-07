using SensoBackend.Application.Modules.Notes;
using SensoBackend.Application.Modules.Notes.Contracts;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Notes;

public sealed class UpdateNoteHandlerTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly UpdateNoteHandler _sut;

    public UpdateNoteHandlerTests() => _sut = new UpdateNoteHandler(_context);

    [Fact]
    public async Task Handle_ShouldUpdateNote_WhenAllConditionsAreSatisfied()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);
        var note = await _context.SetUpNote(seniorAccount);

        var noteDto = await _sut.Handle(
            new UpdateNoteRequest
            {
                AccountId = note.AccountId,
                NoteId = note.Id,
                Dto = new UpsertNoteDto { Content = "Updated note content", IsPrivate = false }
            },
            CancellationToken.None
        );

        noteDto.Content.Should().Be("Updated note content");
        noteDto.CreatedAt.Should().Be(note.CreatedAt);
        noteDto.IsPrivate.Should().BeFalse();
        noteDto.Title.Should().Be(note.Title);
    }

    [Fact]
    public async Task Handle_ShouldThrowNoteNotFoundException_WhenTheNoteIdIsInvalid()
    {
        var action = async () =>
            await _sut.Handle(
                new UpdateNoteRequest
                {
                    AccountId = 15,
                    NoteId = 32,
                    Dto = new UpsertNoteDto { Content = "Some content", IsPrivate = true }
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<NoteNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowNoteAccessDeniedException_WhenAnotherUserTriesToEdit()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);
        var note = await _context.SetUpNote(seniorAccount);
        var impostor = await _context.SetUpAccount();

        var action = async () =>
            await _sut.Handle(
                new UpdateNoteRequest
                {
                    AccountId = impostor.Id,
                    NoteId = note.Id,
                    Dto = new UpsertNoteDto { Content = "New text", IsPrivate = true }
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<NoteAccessDeniedException>();
    }
}

public sealed class UpdateNoteValidatorTests
{
    private readonly UpdateNoteValidator _sut = new();

    [Theory]
    [InlineData("My note", false, null)]
    [InlineData("Lorem ipsum dolor sit amet", true, null)]
    [InlineData("My note", false, "A note can have a title")]
    [InlineData("Lorem ipsum dolor sit amet", true, "Another title")]
    public void Validate_ShouldSucceed_WhenPassedValidRequest(
        string content,
        bool isPrivate,
        string? title
    )
    {
        var request = new UpdateNoteRequest
        {
            AccountId = 1,
            NoteId = 3,
            Dto = new UpsertNoteDto
            {
                Content = content,
                IsPrivate = isPrivate,
                Title = title
            }
        };

        var result = _sut.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenContentIsEmpty()
    {
        var request = new UpdateNoteRequest
        {
            AccountId = 17,
            NoteId = 5,
            Dto = new UpsertNoteDto { Content = string.Empty, IsPrivate = false }
        };

        var result = _sut.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldFail_WhenTitleIsTooLong()
    {
        var request = new UpdateNoteRequest
        {
            AccountId = 1,
            NoteId = 60,
            Dto = new UpsertNoteDto
            {
                Content = "Valid content",
                IsPrivate = true,
                Title = new string('x', 300)
            }
        };

        var result = _sut.Validate(request);

        result.IsValid.Should().BeFalse();
    }
}
