using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.EditCaretakerProfile;
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

    [Fact]
    public async Task Handle_ShouldThrownValidationException_WhenProfileIsSelf()
    {
        var profile = Generators.SeniorProfile.Generate();
        await _context.Profiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        var act = async () =>
            await _sut.Handle(
                new EditCaretakerProfileRequest
                {
                    AccountId = profile.AccountId,
                    SeniorId = profile.AccountId,
                    SeniorAlias = profile.Alias
                },
                CancellationToken.None
            );

        await act.Should().ThrowAsync<ValidationException>();
    }
}

public sealed class EditCaretakerProfileValidatorTests
{
    private readonly EditCaretakerProfileValidator _sut = new();

    [Fact]
    public void Validate_ShouldThrowValidationException_WhenSeniorAliasIsEmpty()
    {
        var dto = Generators.CaretakerProfile.Generate();

        Action act = () =>
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
}
