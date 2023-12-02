using System.Reflection;
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

        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        services.ConfigureOptions<SwaggerGenOptionsSetup>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddAuthorization();
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        services.AddTransient<IJwtProvider, JwtProvider>();
    }
}
