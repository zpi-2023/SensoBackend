using System.ComponentModel.DataAnnotations;
using SensoBackend.Application.Modules.Accounts.Contracts;

namespace SensoBackend.Application.Abstractions;

public sealed record TokenDto
{
    [Required]
    public required string Token { get; init; }
}

public interface IJwtProvider
{
    TokenDto GenerateToken(AccountDto account);
}
