using Microsoft.AspNetCore.Authentication.JwtBearer;
using SensoBackend.Application.Abstractions;
using SensoBackend.WebApi.Authenticaion;
using SensoBackend.WebApi.OptionsSetup;
using Asp.Versioning;
using System.Text.Json.Serialization;

namespace SensoBackend.WebApi;

public static class ServiceExtensions
{
    public static void AddWebApiLayer(this IServiceCollection services)
    {
        services.ConfigureOptions<RouteOptionsSetup>();
        services.AddControllers();
        services.ConfigureOptions<JsonOptionsSetup>();

        services.AddEndpointsApiExplorer();
        var apiVersioningBuilder = services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
        apiVersioningBuilder.AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen();
        services.ConfigureOptions<SwaggerGenOptionsSetup>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddTransient<IJwtProvider, JwtProvider>();
    }
}
