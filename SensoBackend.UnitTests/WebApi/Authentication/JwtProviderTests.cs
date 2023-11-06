using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using SensoBackend.Tests.Utils;
using SensoBackend.UnitTests.Utils;
using SensoBackend.WebApi.Authenticaion;
using SensoBackend.WebApi.Authentication;

namespace SensoBackend.UnitTests.WebApi.Authentication;

public sealed class JwtProviderTests
{
    private static readonly DateTimeOffset Now = new(2020, 5, 10, 20, 30, 0, TimeSpan.Zero);

    private readonly IOptions<JwtOptions> _jwtOptions = Options.Create(
        new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "LongTestSecretKey"
        }
    );

    private readonly JwtProvider _sut;

    public JwtProviderTests() => _sut = new JwtProvider(_jwtOptions, new MockTimeProvider(Now));

    [Fact]
    public void GenerateToken_ShouldReturnToken()
    {
        var account = Generators.AccountDto.Generate();
        var tokenValue = _sut.GenerateToken(account);

        tokenValue.Should().NotBeNull();
        tokenValue.Token.Should().NotBeNullOrEmpty();

        var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue.Token);

        token.ValidTo.Should().Be(Now.UtcDateTime);
        token.Issuer.Should().Be(_jwtOptions.Value.Issuer);
        token.Audiences.Should().Contain(_jwtOptions.Value.Audience);
        token.Claims
            .Should()
            .Contain(c => c.Type == JwtRegisteredClaimNames.Sub)
            .Equals(account.Id);
        token.Claims
            .Should()
            .Contain(c => c.Type == JwtRegisteredClaimNames.Email)
            .Equals(account.Email);
    }
}
