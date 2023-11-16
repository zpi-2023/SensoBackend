using FluentValidation;
using Microsoft.Extensions.Time.Testing;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Profiles;

public sealed class CreateCaretakerProfileHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly TimeProvider _timeProvider = new FakeTimeProvider(
        new DateTimeOffset(new DateTime(2023, 9, 4, 12, 0, 0), TimeSpan.Zero)
    );
    private readonly ISeniorIdRepo _seniorIdRepo;
    private readonly CreateCaretakerProfileHandler _sut;

    public CreateCaretakerProfileHandlerTests()
    {
        _seniorIdRepo = new SeniorIdRepo(_timeProvider);
        _sut = new CreateCaretakerProfileHandler(_context, _seniorIdRepo);
    }

    public void Dispose() => _context.Dispose();

    private int SetUpHash(Account account)
    {
        return _seniorIdRepo.AssignHash(
            new SeniorDataToEncode
            {
                SeniorDisplayName = account.DisplayName,
                SeniorId = account.Id,
                ValidTo = _timeProvider.GetUtcNow().AddMinutes(20)
            }
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowSeniorNotFoundException_WhenCodeIsMissingInRepo()
    {
        var account = await _context.SetUpAccount();

        var action = async () =>
            await _sut.Handle(
                new CreateCaretakerProfileRequest
                {
                    AccountId = account.Id,
                    Hash = 123,
                    SeniorAlias = "Senior"
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<SeniorNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenSeniorIdIsTheSameAsAccountId()
    {
        var account = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(account);
        var hash = SetUpHash(account);

        var action = async () =>
            await _sut.Handle(
                new CreateCaretakerProfileRequest
                {
                    AccountId = account.Id,
                    Hash = hash,
                    SeniorAlias = "Senior"
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowCaretakerProfileAlreadyExistsException_WhenProfileAlreadyExists()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);
        var caretakerAccount = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretakerAccount, seniorAccount);
        var hash = SetUpHash(seniorAccount);

        var action = async () =>
            await _sut.Handle(
                new CreateCaretakerProfileRequest
                {
                    AccountId = caretakerAccount.Id,
                    Hash = hash,
                    SeniorAlias = "Senior"
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<CaretakerProfileAlreadyExistsException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowSeniorNotFoundException_WhenSeniorWasDeleted()
    {
        var seniorAccount = await _context.SetUpAccount();
        var seniorProfile = await _context.SetUpSeniorProfile(seniorAccount);
        var caretakerAccount = await _context.SetUpAccount();
        var hash = SetUpHash(seniorAccount);

        _context.Profiles.Remove(seniorProfile);
        await _context.SaveChangesAsync();

        var action = async () =>
            await _sut.Handle(
                new CreateCaretakerProfileRequest
                {
                    AccountId = caretakerAccount.Id,
                    Hash = hash,
                    SeniorAlias = "Senior"
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<SeniorNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldReturnDto_WhenProfileDoesNotExist()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);
        var caretakerAccount = await _context.SetUpAccount();
        var hash = SetUpHash(seniorAccount);

        var dto = await _sut.Handle(
            new CreateCaretakerProfileRequest
            {
                AccountId = caretakerAccount.Id,
                Hash = hash,
                SeniorAlias = "Custom alias"
            },
            CancellationToken.None
        );

        dto.Type.Should().Be("caretaker");
        dto.SeniorId.Should().Be(seniorAccount.Id);
        dto.SeniorAlias.Should().Be("Custom alias");
    }

    [Fact]
    public async Task Handle_ShouldCreateProfile_WhenProfileDoesNotExist()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);
        var caretakerAccount = await _context.SetUpAccount();
        var hash = SetUpHash(seniorAccount);

        await _sut.Handle(
            new CreateCaretakerProfileRequest
            {
                AccountId = caretakerAccount.Id,
                Hash = hash,
                SeniorAlias = "Custom alias"
            },
            CancellationToken.None
        );

        _context
            .Profiles
            .Any(p => p.AccountId == caretakerAccount.Id && p.SeniorId == seniorAccount.Id)
            .Should()
            .BeTrue();
    }
}
