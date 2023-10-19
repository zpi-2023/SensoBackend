using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Configuration;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles"); //may want to change it later to some static value

        builder.HasKey(x => x.Id);

        builder.HasData(
            Role.Admin,
            Role.Member);
    }
}
