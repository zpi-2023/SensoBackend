using SensoBackend.Application.Modules.Profiles.Utils;

namespace SensoBackend.Application.Abstractions;

public interface IAuthorizationService
{
    Task<int> GetRoleIdAsync(int accountId);
    Task<List<ProfileInfo>> GetProfilesByAccountId(int accountId);
}
