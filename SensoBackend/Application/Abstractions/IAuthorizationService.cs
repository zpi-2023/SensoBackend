using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Enums;

namespace SensoBackend.Application.Abstractions;

public interface IAuthorizationService
{
    Task<Role> GetRoleAsync(int accountId);
    Task<List<ProfileInfo>> GetProfilesByAccountId(int accountId);
}
