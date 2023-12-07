using SensoBackend.Domain.Enums;

namespace SensoBackend.WebApi.Authorization.Data;

public static class RolePermission
{
    private static HashSet<Permission> MemberPermissions =>

        [
            Permission.ManageAccount,
            Permission.ManageDashboard,
            Permission.ManageGames,
            Permission.ManageProfiles,
            Permission.ManageReminders,
            Permission.MutateNotes,
            Permission.ReadNotes,
            Permission.ReadAlerts,
            Permission.MutateAlerts
        ];

    private static HashSet<Permission> AdminPermissions => [.. Enum.GetValues<Permission>()];

    public static HashSet<Permission> GetPermissions(this Role role) =>
        role switch
        {
            Role.Admin => AdminPermissions,
            Role.Member => MemberPermissions,
            _ => throw new InvalidDataException("Provided role does not exist")
        };
}
