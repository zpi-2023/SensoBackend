using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Configuration;

[UsedImplicitly]
internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email).HasMaxLength(255);

        builder.Property(x => x.Password).HasMaxLength(72);

        builder.Property(x => x.DisplayName).HasMaxLength(255);

        builder.Property(x => x.PhoneNumber).HasMaxLength(9);
    }
}
