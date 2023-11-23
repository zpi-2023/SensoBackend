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
        var account = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(account);

        await _sut.Handle(
            new DeleteSeniorProfileRequest { AccountId = account.Id },
            CancellationToken.None
        );

        var deletedProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == account.Id);

        deletedProfile.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrowProfileNotFoundException_WhenProfileDoesNotExist()
    {
        var account = await _context.SetUpAccount();

        var act = async () =>
            await _sut.Handle(
                new DeleteSeniorProfileRequest { AccountId = account.Id },
                CancellationToken.None
            );

        await act.Should().ThrowAsync<ProfileNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowRemoveSeniorProfileDeniedException_WhenProfileHasCaretakerProfiles()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);

        var caretakerAccount = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretakerAccount, seniorAccount);

        var act = async () =>
            await _sut.Handle(
                new DeleteSeniorProfileRequest { AccountId = seniorAccount.Id },
                CancellationToken.None
            );

        await act.Should().ThrowAsync<RemoveSeniorProfileDeniedException>();
    }
}
