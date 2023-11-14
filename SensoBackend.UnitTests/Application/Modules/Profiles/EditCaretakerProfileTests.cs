using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Profiles.EditCaretakerProfile;

public sealed class EditCaretakerProfileHandlerTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly EditCaretakerProfileHandler _sut;

    public EditCaretakerProfileHandlerTests() => _sut = new EditCaretakerProfileHandler(_context);

    [Fact]
    public async Task Handle_ShouldEditProfile()
    {
        var profile = Generators.CaretakerProfile.Generate();
        await _context.Profiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        var dto = Generators.EditCaretakerProfileDto.Generate();

        await _sut.Handle(
            new EditCaretakerProfileRequest
            {
                AccountId = profile.AccountId,
                SeniorId = profile.SeniorId,
                SeniorAlias = dto.SeniorAlias
            },
            CancellationToken.None
        );

        var editedProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profile.Id);

        editedProfile.Should().NotBeNull();
        if (editedProfile == null)
            return;

        editedProfile.Alias.Should().Be(dto.SeniorAlias);
    }

    [Fact]
    public async Task Handle_ShouldThrowProfileNotFoundException_WhenProfileDoesNotExist()
    {
        var profile = Generators.CaretakerProfile.Generate();

        var act = async () =>
            await _sut.Handle(
                new EditCaretakerProfileRequest
                {
                    AccountId = profile.AccountId,
                    SeniorId = profile.SeniorId,
                    SeniorAlias = profile.Alias
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
        var dto = Generators.CaretakerProfile.Generate();

        var act = () =>
            _sut.ValidateAndThrow(
                new EditCaretakerProfileRequest
                {
                    AccountId = dto.AccountId,
                    SeniorId = dto.SeniorId,
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
            AccountId = 1,
            SeniorId = 1,
            SeniorAlias = ""
        };

        var act = () => _sut.ValidateAndThrow(request);

        act.Should().Throw<ValidationException>();
    }
}
