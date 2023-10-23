using SensoBackend.Application.Modules.Profiles.AdditionalModels;

namespace SensoBackend.Application.Abstractions;

public interface IAuthorizationService
{
    Task<int> GetRoleIdAsync(int accountId);
    Task<List<ProfileDto>> GetProfilesByAccountId(int accountId);
}
