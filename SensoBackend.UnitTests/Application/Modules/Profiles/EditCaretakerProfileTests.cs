using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Profiles.EditCaretakerProfile;

public sealed class EditCaretakerProfileHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly EditCaretakerProfileHandler _sut;

    public EditCaretakerProfileHandlerTests() => _sut = new EditCaretakerProfileHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldEditProfile()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);

        var caretakerAccount = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretakerAccount, seniorAccount);

        var dto = Generators.EditCaretakerProfileDto.Generate();

        await _sut.Handle(
            new EditCaretakerProfileRequest
            {
                AccountId = caretakerAccount.Id,
                SeniorId = seniorAccount.Id,
                SeniorAlias = dto.SeniorAlias
            },
            CancellationToken.None
        );

        var editedProfile = await _context
            .Profiles
            .FirstOrDefaultAsync(
                p => p.AccountId == caretakerAccount.Id && p.SeniorId == seniorAccount.Id
            );

        editedProfile.Should().NotBeNull();
        if (editedProfile == null)
            return;
        editedProfile.Alias.Should().Be(dto.SeniorAlias);
    }

    [Fact]
    public async Task Handle_ShouldThrowProfileNotFoundException_WhenProfileDoesNotExist()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);

        var caretakerAccount = await _context.SetUpAccount();

        var dto = Generators.EditCaretakerProfileDto.Generate();

        var act = async () =>
            await _sut.Handle(
                new EditCaretakerProfileRequest
                {
                    AccountId = caretakerAccount.Id,
                    SeniorId = seniorAccount.Id,
                    SeniorAlias = dto.SeniorAlias
                },
                CancellationToken.None
            );

        await act.Should().ThrowAsync<ProfileNotFoundException>();
    }
}

public sealed class EditCaretakerProfileValidatorTests
{
    private readonly EditCaretakerProfileValidator _sut = new();

    [Fact]
    public void Validate_ShouldThrowValidationException_WhenSeniorAliasIsEmpty()
    {
        var act = () =>
            _sut.ValidateAndThrow(
                new EditCaretakerProfileRequest
                {
                    AccountId = 0,
                    SeniorId = 1,
                    SeniorAlias = string.Empty
                }
            );

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ShouldThrowValidationException_WhenAccountIdIsEqualSeniorId()
    {
        var request = new EditCaretakerProfileRequest
        {
            AccountId = 0,
            SeniorId = 0,
            SeniorAlias = ""
        };

        var act = () => _sut.ValidateAndThrow(request);

        act.Should().Throw<ValidationException>();
    }
}
