using SensoBackend.Application.Modules.Accounts.Contracts;

namespace SensoBackend.Application.Abstractions;

public interface IJwtProvider
{
    string GenerateToken(AccountDto account);
}
