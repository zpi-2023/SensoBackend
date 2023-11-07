using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.UnitTests.Utils;

public static class Database
{
    public static AppDbContext CreateFixture()
    {
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .ConfigureWarnings(
                warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)
            )
            .Options;

        var context = new AppDbContext(contextOptions);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }
}
