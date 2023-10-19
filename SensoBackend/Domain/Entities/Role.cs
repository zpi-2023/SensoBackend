
using RoleEnum = SensoBackend.WebApi.Authorization.Data.Role;

namespace SensoBackend.Domain.Entities;

public sealed class Role
{
    public static readonly Role Admin = new((int)RoleEnum.Admin, RoleEnum.Admin.ToString());
    public static readonly Role Member = new((int)RoleEnum.Member, RoleEnum.Member.ToString());

    public int Id { get; set; }
    public string Name { get; set; }

    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

}
