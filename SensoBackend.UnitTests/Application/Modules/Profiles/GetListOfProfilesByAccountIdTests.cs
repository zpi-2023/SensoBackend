using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Profiles;

public sealed class GetListOfProfilesByAccountIdHandlerTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetListOfProfilesByAccountIdHandler _sut;

    public GetListOfProfilesByAccountIdHandlerTests() => _sut = new(_context);

    private async Task CreateCaretakerProfile(Account owner)
    {
        await _context.SetUpCaretakerProfile(owner, await _context.SetUpAccount());
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfProfiles_WhenGivenAccountId()
    {
        var account = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(account);
        await CreateCaretakerProfile(account);
        await CreateCaretakerProfile(account);
        await CreateCaretakerProfile(account);

        var profiles = await _sut.Handle(
            new GetListOfProfilesByAccountIdRequest { AccountId = account.Id },
            CancellationToken.None
        );

        profiles.Should().HaveCount(4);
    }
}
