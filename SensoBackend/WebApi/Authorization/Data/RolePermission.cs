using SensoBackend.Domain.Entities;

namespace SensoBackend.WebApi.Authorization.Data;

public static class RolePermission
{
    private static HashSet<Permission> MemberPermissions =>
        new()
        {
            Permission.ManageProfiles,
            Permission.ManageDashboard,
            Permission.ReadNotes,
            Permission.MutateNotes
        };

    private static HashSet<Permission> AdminPermissions => Enum.GetValues<Permission>().ToHashSet();

    public static HashSet<Permission> GetPermissions(this Role role) =>
        role.Id switch
        {
            Role.AdminId => AdminPermissions,
            Role.MemberId => MemberPermissions,
            _ => throw new InvalidDataException("Provided role does not exist")
        };
}
