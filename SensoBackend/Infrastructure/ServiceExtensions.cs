using Microsoft.EntityFrameworkCore;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Infrastructure;

public static class ServiceExtensions
{
    public static void AddInfrastructureLayer(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
    }
}
