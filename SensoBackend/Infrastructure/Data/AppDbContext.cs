using Microsoft.EntityFrameworkCore;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Infrastructure.Data;

public class AppDbContext : DbContext
{
    protected AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<User> Users => Set<User>();
<<<<<<< HEAD:SensoBackend/Infrastructure/Data/AppDbContext.cs
=======
    public virtual DbSet<Account> Accounts => Set<Account>();
>>>>>>> 51c84df (add account model and jwt options):SensoBackend/Data/AppDbContext.cs
}
