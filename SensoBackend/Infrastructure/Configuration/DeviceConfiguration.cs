using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Configuration;

[UsedImplicitly]
internal sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token).HasMaxLength(255);

        builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId);

        builder.HasIndex(x => x.AccountId).IsUnique();
    }
}
