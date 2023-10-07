using Microsoft.EntityFrameworkCore;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Infrastructure;

public static class AppExtensions
{
    public static void AutoMigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
}
