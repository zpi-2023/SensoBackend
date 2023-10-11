using Asp.Versioning;
using Microsoft.Extensions.Options;

namespace SensoBackend.WebApi.OptionsSetup;

public class ApiVersioningOptionsSetup : IConfigureOptions<ApiVersioningOptions>
{
    public void Configure(ApiVersioningOptions options)
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    }
}
