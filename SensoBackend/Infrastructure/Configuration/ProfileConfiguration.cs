using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Configuration;

[UsedImplicitly]
internal sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId);

        builder.HasOne(x => x.Senior).WithMany().HasForeignKey(x => x.SeniorId);
    }
}
