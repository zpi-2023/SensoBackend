using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profile = SensoBackend.Domain.Entities.Profile;

namespace SensoBackend.Infrastructure.Configuration;

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
