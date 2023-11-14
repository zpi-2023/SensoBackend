using SensoBackend.Application.Modules.Notes;
using SensoBackend.Application.Modules.Notes.Contracts;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Notes;

public sealed class CreateNoteHandlerTests
{
    private static readonly DateTimeOffset Now = new(2015, 4, 12, 18, 38, 5, TimeSpan.Zero);
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly CreateNoteHandler _sut;

    public CreateNoteHandlerTests() =>
        _sut = new CreateNoteHandler(_context, new MockTimeProvider { Now = Now });

    [Fact]
    public async Task Handle_ShouldCreateNote_WhenAllConditionsAreSatisfied()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);

        var noteDto = await _sut.Handle(
            new CreateNoteRequest
            {
                AccountId = seniorAccount.Id,
                Dto = new UpsertNoteDto { Content = "my note content", IsPrivate = false }
            },
            CancellationToken.None
        );

        noteDto.Content.Should().Be("my note content");
        noteDto.CreatedAt.Should().Be(Now);
        noteDto.IsPrivate.Should().BeFalse();
        noteDto.Title.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrowSeniorNotFoundException_WhenSeniorProfileIsNotPresent()
    {
        var invalidAccount = await _context.SetUpAccount();

        var action = async () =>
            await _sut.Handle(
                new CreateNoteRequest
                {
                    AccountId = invalidAccount.Id,
                    Dto = new UpsertNoteDto { Content = "some contents", IsPrivate = true }
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<SeniorNotFoundException>();
    }
}

public sealed class CreateNoteValidatorTests
{
    private readonly CreateNoteValidator _sut = new();

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
        var request = new CreateNoteRequest
        {
            AccountId = 1,
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
        var request = new CreateNoteRequest
        {
            AccountId = 6,
            Dto = new UpsertNoteDto { Content = string.Empty, IsPrivate = false }
        };

        var result = _sut.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldFail_WhenTitleIsTooLong()
    {
        var request = new CreateNoteRequest
        {
            AccountId = 15,
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
