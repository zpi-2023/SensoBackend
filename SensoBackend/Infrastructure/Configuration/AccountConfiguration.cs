using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensoBackend.Domain.Entities;
using Account = SensoBackend.Domain.Entities.Account;

namespace SensoBackend.Infrastructure.Configuration;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts"); //may want to change it later to some static value

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId);

        builder.HasData(
            Create(1, "admin@senso.pl", "admin_senso", Role.Admin),
            Create(2, "senior@senso.pl", "senior_senso", Role.Member),
            Create(3, "caretaker@senso.pl", "caretaker_senso", Role.Member));
    }

    private Account Create(int id, string email, string password, Role role)
    {
        var time = new DateTimeOffset(DateTime.UtcNow);
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var account = new Account{
            Id = id,
            Email = email,
            Password = hashedPassword,
            Active = true,
            Verified = true,
            PhoneNumber = "123456789",
            DisplayName = password,
            CreatedAt = time,
            LastLoginAt = time,
            LastPasswordChangeAt = time,
            RoleId = role.Id
        };

        return account;
    }
}
