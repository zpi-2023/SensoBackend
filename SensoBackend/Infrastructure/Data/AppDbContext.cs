using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Configuration;

namespace SensoBackend.Infrastructure.Data;

public class AppDbContext : DbContext
{
    protected AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<Account> Accounts => Set<Account>();
    public virtual DbSet<Role> Roles => Set<Role>();
    public virtual DbSet<Profile> Profiles => Set<Profile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileConfiguration());
    }
}
