using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Profiles.DeleteCaretakerProfile;

public sealed class DeleteCaretakerProfileHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly DeleteCaretakerProfileHandler _sut;

    public DeleteCaretakerProfileHandlerTests() =>
        _sut = new DeleteCaretakerProfileHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldDeleteProfile_AsCaretaker()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);

        var caretakerAccount = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretakerAccount, seniorAccount);

        await _sut.Handle(
            new DeleteCaretakerProfileRequest
            {
                AccountId = caretakerAccount.Id,
                SeniorId = seniorAccount.Id,
                CaretakerId = caretakerAccount.Id
            },
            CancellationToken.None
        );

        var deletedProfile = await _context
            .Profiles
            .FirstOrDefaultAsync(p => p.Id == caretakerAccount.Id);

        deletedProfile.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldDeleteProfile_AsSenior()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);

        var caretakerAccount = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretakerAccount, seniorAccount);

        await _sut.Handle(
            new DeleteCaretakerProfileRequest
            {
                AccountId = seniorAccount.Id,
                SeniorId = seniorAccount.Id,
                CaretakerId = caretakerAccount.Id
            },
            CancellationToken.None
        );

        var deletedProfile = await _context
            .Profiles
            .FirstOrDefaultAsync(p => p.Id == caretakerAccount.Id);

        deletedProfile.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrowProfileNotFoundException_WhenProfileDoesNotExist_AsCaretaker()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);

        var caretakerAccount = await _context.SetUpAccount();

        var act = async () =>
            await _sut.Handle(
                new DeleteCaretakerProfileRequest
                {
                    AccountId = caretakerAccount.Id,
                    SeniorId = seniorAccount.Id,
                    CaretakerId = caretakerAccount.Id
                },
                CancellationToken.None
            );

        await act.Should().ThrowAsync<ProfileNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowProfileNotFoundException_WhenProfileDoesNotExist_AsSenior()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);

        var caretakerAccount = await _context.SetUpAccount();

        var act = async () =>
            await _sut.Handle(
                new DeleteCaretakerProfileRequest
                {
                    AccountId = seniorAccount.Id,
                    SeniorId = seniorAccount.Id,
                    CaretakerId = caretakerAccount.Id
                },
                CancellationToken.None
            );

        await act.Should().ThrowAsync<ProfileNotFoundException>();
    }
}

public sealed class DeleteCaretakerProfileValidatorTests
{
    private readonly DeleteCaretakerProfileValidator _sut = new();

    [Fact]
    public void Validate_ShouldThrowValidationException_WhenAccountIdIsNotEqualSeniorIdOrCaretakerId()
    {
        var request = new DeleteCaretakerProfileRequest
        {
            AccountId = 0,
            SeniorId = 1,
            CaretakerId = 2
        };

        var act = () => _sut.ValidateAndThrow(request);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ShouldNotThrow_WhenAccountIdIsEqualSeniorId()
    {
        var request = new DeleteCaretakerProfileRequest
        {
            AccountId = 1,
            SeniorId = 1,
            CaretakerId = 2
        };

        var act = () => _sut.ValidateAndThrow(request);

        act.Should().NotThrow<ValidationException>();
    }

    [Fact]
    public void Validate_ShouldNotThrow_WhenAccountIdIsEqualCaretakerId()
    {
        var request = new DeleteCaretakerProfileRequest
        {
            AccountId = 2,
            SeniorId = 1,
            CaretakerId = 2
        };

        var act = () => _sut.ValidateAndThrow(request);

        act.Should().NotThrow<ValidationException>();
    }

    [Fact]
    public void Validate_ShouldThrow_WhenSeniorIdIsEqualCaretakerId()
    {
        var request = new DeleteCaretakerProfileRequest
        {
            AccountId = 1,
            SeniorId = 1,
            CaretakerId = 1
        };

        var act = () => _sut.ValidateAndThrow(request);

        act.Should().Throw<ValidationException>();
    }
}
