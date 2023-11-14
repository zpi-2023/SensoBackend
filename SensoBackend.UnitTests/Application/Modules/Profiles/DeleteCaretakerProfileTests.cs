using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Profiles.DeleteCaretakerProfile;

public sealed class DeleteCaretakerProfileHandlerTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly DeleteCaretakerProfileHandler _sut;

    public DeleteCaretakerProfileHandlerTests() =>
        _sut = new DeleteCaretakerProfileHandler(_context);

    [Fact]
    public async Task Handle_ShouldDeleteProfile()
    {
        var profile = Generators.CaretakerProfile.Generate();
        await _context.Profiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        await _sut.Handle(
            new DeleteCaretakerProfileRequest
            {
                AccountId = profile.AccountId,
                SeniorId = profile.SeniorId
            },
            CancellationToken.None
        );

        var deletedProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profile.Id);

        deletedProfile.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrowProfileNotFoundException_WhenProfileDoesNotExist()
    {
        var profile = Generators.CaretakerProfile.Generate();

        var act = async () =>
            await _sut.Handle(
                new DeleteCaretakerProfileRequest
                {
                    AccountId = profile.AccountId,
                    SeniorId = profile.SeniorId
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
    public void Validate_ShouldThrowValidationException_WhenAccountIdIsEqualSeniorId()
    {
        var request = new DeleteCaretakerProfileRequest { AccountId = 1, SeniorId = 1 };

        var act = () => _sut.ValidateAndThrow(request);

        act.Should().Throw<ValidationException>();
    }
}
