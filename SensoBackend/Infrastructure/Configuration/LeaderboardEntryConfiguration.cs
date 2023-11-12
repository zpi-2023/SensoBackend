using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Configuration;

[UsedImplicitly]
internal sealed class LeaderboardEntryConfiguration : IEntityTypeConfiguration<LeaderboardEntry>
{
    public void Configure(EntityTypeBuilder<LeaderboardEntry> builder)
    {
        builder.ToTable("LeaderboardEntries");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId);
    }
}
