using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Data;

public class AppDbContext : DbContext
{
    protected AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<Account> Accounts => Set<Account>();
    public virtual DbSet<Role> Roles => Set<Role>();
    public virtual DbSet<Profile> Profiles => Set<Profile>();
    public virtual DbSet<Gadget> Gadgets => Set<Gadget>();
    public virtual DbSet<DashboardItem> DashboardItems => Set<DashboardItem>();
    public virtual DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
