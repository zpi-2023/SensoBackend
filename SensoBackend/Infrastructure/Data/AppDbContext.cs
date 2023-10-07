using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<User> Users => Set<User>();
}
