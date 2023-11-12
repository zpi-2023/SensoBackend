using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.DeleteCaretakerProfile;
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

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenProfileIsSelf()
    {
        var profile = Generators.SeniorProfile.Generate();
        await _context.Profiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        var act = async () =>
            await _sut.Handle(
                new DeleteCaretakerProfileRequest
                {
                    AccountId = profile.AccountId,
                    SeniorId = profile.SeniorId
                },
                CancellationToken.None
            );

        await act.Should().ThrowAsync<ValidationException>();
    }
}
