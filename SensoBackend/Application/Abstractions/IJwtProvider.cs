using SensoBackend.Application.Modules.Accounts.Contracts;

namespace SensoBackend.Application.Abstractions;

public sealed record TokenDto(string Token);

public interface IJwtProvider
{
    TokenDto GenerateToken(AccountDto account);
}
