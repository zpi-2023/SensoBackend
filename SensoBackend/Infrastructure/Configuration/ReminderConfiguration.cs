using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Configuration;

[UsedImplicitly]
internal sealed class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.ToTable("Reminders");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Senior).WithMany().HasForeignKey(x => x.SeniorId);

        builder.HasOne(x => x.Medication).WithMany().HasForeignKey(x => x.MedicationId);

        builder.Property(x => x.Cron).HasMaxLength(255);

        builder.Property(x => x.Description).HasColumnType("text");
    }
}
