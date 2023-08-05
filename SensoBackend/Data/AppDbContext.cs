using Microsoft.EntityFrameworkCore;
using SensoBackend.Models;

namespace SensoBackend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<User> Users { get; set; }
}
