using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Application.Modules.Accounts.GetAccountById;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Accounts.GetAccountByCredentials;

public sealed class GetAccountTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetAccountHandler _sut;

    public GetAccountTests() => _sut = new GetAccountHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnAccountDto()
    {
        var account = await _context.SetUpAccount();
        var expected = new GetAccountDto
        {
            Id = account.Id,
            Email = account.Email,
            DisplayName = account.DisplayName,
            PhoneNumber = account.PhoneNumber
        };

        var accountDto = await _sut.Handle(
            new GetAccountRequest { AccountId = account.Id },
            CancellationToken.None
        );

        accountDto.Should().NotBeNull();
        accountDto.Should().BeOfType<GetAccountDto>();
        accountDto.Should().BeEquivalentTo(expected);
    }
}
