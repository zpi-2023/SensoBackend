using SensoBackend.Domain.Entities;

namespace SensoBackend.WebApi.Authorization.Data;

public static class RolePermission
{
    private static HashSet<Permission> MemberPermissions
        => new HashSet<Permission> {
            Permission.Read,
            Permission.Write,
        };

    private static HashSet<Permission> AdminPermissions
        => Enum.GetValues<Permission>().ToHashSet(); //admin can do anything

    public static HashSet<Permission> GetPermissions(this Role role)
        => role.Id switch
        {
            Role.AdminId => AdminPermissions,
            Role.MemberId => MemberPermissions,
            _ => throw new InvalidDataException("Provided role does not exist")
        };

}
