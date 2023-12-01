using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.WebApi.Authentication;

namespace SensoBackend.WebApi.Authenticaion;

public sealed class JwtProvider(IOptions<JwtOptions> options, TimeProvider timeProvider)
    : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateToken(AccountDto account)
    {
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, account.Email)
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            timeProvider.GetUtcNow().UtcDateTime.AddDays(7),
            signingCredentials
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }
}
