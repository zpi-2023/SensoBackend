using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Profiles.DeleteSeniorProfile;

public sealed class DeleteSeniorProfileHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly DeleteSeniorProfileHandler _sut;

    public DeleteSeniorProfileHandlerTests() => _sut = new DeleteSeniorProfileHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldDeleteProfile()
    {
        var profile = Generators.SeniorProfile.Generate();
        await _context.Profiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        await _sut.Handle(
            new DeleteSeniorProfileRequest(profile.AccountId),
            CancellationToken.None
        );

        var deletedProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profile.Id);

        deletedProfile.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrowProfileNotFoundException_WhenProfileDoesNotExist()
    {
        var profile = Generators.SeniorProfile.Generate();

        var act = async () =>
            await _sut.Handle(
                new DeleteSeniorProfileRequest(profile.AccountId),
                CancellationToken.None
            );

        await act.Should().ThrowAsync<ProfileNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowRemoveSeniorProfileDeniedException_WhenProfileHasCaretakerProfiles()
    {
        var profile = Generators.SeniorProfile.Generate();
        await _context.Profiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        var caretakerProfile = Generators.CaretakerProfile.Generate();
        await _context.Profiles.AddAsync(caretakerProfile);
        await _context.SaveChangesAsync();

        var act = async () =>
            await _sut.Handle(
                new DeleteSeniorProfileRequest(profile.AccountId),
                CancellationToken.None
            );

        await act.Should().ThrowAsync<RemoveSeniorProfileDeniedException>();
    }
}
