using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Configuration;

[UsedImplicitly]
internal sealed class DashboardItemConfiguration : IEntityTypeConfiguration<DashboardItem>
{
    public void Configure(EntityTypeBuilder<DashboardItem> builder)
    {
        builder.ToTable("DashboardItems");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Gadget).WithMany().HasForeignKey(x => x.GadgetId);

        builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId);
    }
}
