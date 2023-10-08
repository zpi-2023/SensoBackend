using Microsoft.EntityFrameworkCore;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Tests.Utils;

public static class Database
{
    public static AppDbContext CreateFixture()
    {
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        var context = new AppDbContext(contextOptions);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }
}
