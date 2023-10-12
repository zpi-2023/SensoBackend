using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.WebApi.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SensoBackend.WebApi.Authenticaion;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options) => _options = options.Value;

    public TokenDto GenerateToken(AccountDto account)
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
            DateTime.UtcNow.AddDays(7),
            signingCredentials
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenDto(tokenValue);
    }
}