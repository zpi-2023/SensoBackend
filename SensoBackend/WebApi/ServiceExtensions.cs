using Microsoft.AspNetCore.Authentication.JwtBearer;
using SensoBackend.Application.Abstractions;
using SensoBackend.WebApi.Authenticaion;
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
        services.AddSwaggerGen();
        services.ConfigureOptions<SwaggerGenOptionsSetup>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddTransient<IJwtProvider, JwtProvider>();
    }
}
