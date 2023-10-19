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

        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId);

        builder.HasOne(x => x.Senior)
            .WithMany()
            .HasForeignKey(x => x.SeniorId);

        builder.HasData(
            Create(1, 2, 2),
            Create(2, 3, 2, "Senior"));
    }

    private Profile Create(int id, int accountId, int seniorId, string? alias = null)
        => new Profile
        {
            Id = id,
            AccountId = accountId,
            SeniorId = seniorId,
            Alias = alias
        };
}
