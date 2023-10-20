namespace SensoBackend.Domain.Entities;

public sealed class Role
{
    public static readonly Role Admin = new(AdminId, "Admin");
    public static readonly Role Member = new(MemberId, "Member");

    public const int AdminId = 1;
    public const int MemberId = 2;

    public int Id { get; set; }
    public string Name { get; set; }

    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
