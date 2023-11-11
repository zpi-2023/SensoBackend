using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Data;

public class AppDbContext : DbContext
{
    protected AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<Account> Accounts => Set<Account>();
    public virtual DbSet<Profile> Profiles => Set<Profile>();
    public virtual DbSet<DashboardItem> DashboardItems => Set<DashboardItem>();
    public virtual DbSet<Note> Notes => Set<Note>();
    public virtual DbSet<Medication> Medications => Set<Medication>();
    public virtual DbSet<Reminder> Reinders => Set<Reminder>();
    public virtual DbSet<IntakeRecord> IntakeRecords => Set<IntakeRecord>();
    public virtual DbSet<LeaderboardEntry> LeaderboardEntries => Set<LeaderboardEntry>();
    public virtual DbSet<Alert> Alerts => Set<Alert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
