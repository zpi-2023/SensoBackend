using Microsoft.Extensions.Options;

namespace SensoBackend.WebApi.OptionsSetup;

public class RouteOptionsSetup : IConfigureOptions<RouteOptions>
{
    public void Configure(RouteOptions options)
    {
        options.LowercaseUrls = true;
    }
}
