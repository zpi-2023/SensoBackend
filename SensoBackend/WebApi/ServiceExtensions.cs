using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SensoBackend.Application.Abstractions;
using SensoBackend.WebApi.Authenticaion;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.OptionsSetup;

namespace SensoBackend.WebApi;

public static class ServiceExtensions
{
    public static void AddWebApiLayer(this IServiceCollection services)
    {
        services.ConfigureOptions<RouteOptionsSetup>();

        services.AddControllers();
        services.ConfigureOptions<JsonOptionsSetup>();

        services.AddEndpointsApiExplorer();
        services.AddApiVersioning().AddApiExplorer();
        services.ConfigureOptions<ApiVersioningOptionsSetup>();
        services.ConfigureOptions<ApiExplorerOptionsSetup>();

        services.AddSwaggerGen();
        services.ConfigureOptions<SwaggerGenOptionsSetup>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddAuthorization();
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        services.AddTransient<IJwtProvider, JwtProvider>();

        services.AddHangfire(
            c =>
                c.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseInMemoryStorage()
        );

        services.AddHangfireServer();
    }
}
