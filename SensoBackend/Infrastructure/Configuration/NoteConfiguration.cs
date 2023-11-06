using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensoBackend.Domain.Entities;

internal sealed class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("Notes");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId);

        builder.Property(x => x.Content).HasColumnType("text");

        builder.Property(x => x.Title).HasMaxLength(255);
    }
}
