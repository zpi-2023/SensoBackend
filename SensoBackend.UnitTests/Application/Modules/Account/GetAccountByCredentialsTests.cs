using Mapster;
using SensoBackend.Application.Modules.Accounts.GetAccountByCredentials;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;
using System.Security.Authentication;

namespace SensoBackend.Tests.Application.Modules.Accounts.GetAccountByCredentials;

public sealed class GetAccountByCredentialsTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetAccountByCredentialsHandler _sut;

    public GetAccountByCredentialsTests() => _sut = new GetAccountByCredentialsHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnAccount_WhenCredentialsAreValid()
    {
        var expectedResult = Generators.AccountDto.Generate() with
        {
            Password = BCrypt.Net.BCrypt.HashPassword("test")
        };

        _context.Accounts.Add(expectedResult.Adapt<Domain.Entities.Account>());
        _context.SaveChanges();

        var dto = Generators.GetAccountByCredentialsDto.Generate() with
        {
            Email = expectedResult.Email,
            Password = "test"
        };

        var result = await _sut.Handle(
            new GetAccountByCredentialsRequest { Dto = dto },
            CancellationToken.None
        );

        result.Should().NotBeNull();
        if (result == null)
            return;

        result.Email.Should().Be(expectedResult.Email);
        result.Password.Should().Be(expectedResult.Password);
        result.PhoneNumber.Should().Be(expectedResult.PhoneNumber);
        result.DisplayName.Should().Be(expectedResult.DisplayName);
        result.CreatedAt.Should().BeCloseTo(expectedResult.CreatedAt, TimeSpan.FromSeconds(2));
        result.LastLoginAt.Should().BeCloseTo(expectedResult.LastLoginAt, TimeSpan.FromSeconds(2));
        result.LastPasswordChangeAt
            .Should()
            .BeCloseTo(expectedResult.LastPasswordChangeAt, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidCredentialException_WhenEmailIsInvalid()
    {
        var dto = Generators.GetAccountByCredentialsDto.Generate();

        var act = async () =>
            await _sut.Handle(
                new GetAccountByCredentialsRequest { Dto = dto },
                CancellationToken.None
            );

        await act.Should().ThrowAsync<InvalidCredentialException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenPasswordIsInvalid()
    {
        var expectedResult = Generators.AccountDto.Generate() with
        {
            Password = BCrypt.Net.BCrypt.HashPassword("test")
        };

        _context.Accounts.Add(expectedResult.Adapt<Domain.Entities.Account>());
        _context.SaveChanges();

        var dto = Generators.GetAccountByCredentialsDto.Generate() with
        {
            Email = expectedResult.Email,
            Password = ""
        };

        var act = async () =>
            await _sut.Handle(
                new GetAccountByCredentialsRequest { Dto = dto },
                CancellationToken.None
            );

        await act.Should().ThrowAsync<InvalidCredentialException>();
    }
}
