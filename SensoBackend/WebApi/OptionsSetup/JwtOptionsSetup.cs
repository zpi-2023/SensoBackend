using Microsoft.Extensions.Options;
using SensoBackend.WebApi.Authentication;

namespace SensoBackend.WebApi.OptionsSetup;

public class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtOptions>
{
    private const string SectionName = "JwtSettings";

    public void Configure(JwtOptions options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}
