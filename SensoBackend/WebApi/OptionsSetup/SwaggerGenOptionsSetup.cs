using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SensoBackend.WebApi.OptionsSetup;

public class SwaggerGenOptionsSetup(IApiVersionDescriptionProvider provider)
    : IConfigureNamedOptions<SwaggerGenOptions>
{
    public void Configure(string? name, SwaggerGenOptions options) => Configure(options);

    public void Configure(SwaggerGenOptions options)
    {
        options.SupportNonNullableReferenceTypes();
        options.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Get token by request to api/v1/token endpoint. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            }
        );
        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            }
        );

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
        }

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    }

    private OpenApiInfo CreateVersionInfo(ApiVersionDescription desc)
    {
        var info = new OpenApiInfo() { Title = "Senso", Version = desc.ApiVersion.ToString() };

        if (desc.IsDeprecated)
        {
            info.Description += "This API version has been deprecated. Please use newer version.";
        }

        return info;
    }
}
