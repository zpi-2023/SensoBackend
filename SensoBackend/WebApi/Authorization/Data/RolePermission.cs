using SensoBackend.Domain.Enums;

namespace SensoBackend.WebApi.Authorization.Data;

public static class RolePermission
{
    private static HashSet<Permission> MemberPermissions =>
        new()
        {
            Permission.ManageProfiles,
            Permission.ManageDashboard,
            Permission.ReadNotes,
            Permission.MutateNotes,
            Permission.ManageGames
        };

    private static HashSet<Permission> AdminPermissions => Enum.GetValues<Permission>().ToHashSet();

    public static HashSet<Permission> GetPermissions(this Role role) =>
        role switch
        {
            Role.Admin => AdminPermissions,
            Role.Member => MemberPermissions,
            _ => throw new InvalidDataException("Provided role does not exist")
        };
}
