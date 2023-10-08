using System.Text.Json.Serialization;

namespace SensoBackend.WebApi;

public static class ServiceExtensions
{
    public static void AddWebApiLayer(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services
            .AddControllers()
            .AddJsonOptions(
                options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
            );
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => options.SupportNonNullableReferenceTypes());
    }
}
