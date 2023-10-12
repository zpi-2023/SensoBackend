using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Data;

public class AppDbContext : DbContext
{
    protected AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<Account> Accounts => Set<Account>();
}
