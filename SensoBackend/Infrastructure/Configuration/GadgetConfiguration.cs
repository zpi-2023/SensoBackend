using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Configuration;

[UsedImplicitly]
internal sealed class GadgetConfiguration : IEntityTypeConfiguration<Gadget>
{
    public void Configure(EntityTypeBuilder<Gadget> builder)
    {
        builder.ToTable("Gadgets");

        builder.HasKey(x => x.Id);

        builder.HasData(Gadget.List);
    }
}
