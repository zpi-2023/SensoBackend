using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;

namespace SensoBackend.WebApi.OptionsSetup;

public class ApiExplorerOptionsSetup : IConfigureOptions<ApiExplorerOptions>
{
    public void Configure(ApiExplorerOptions options)
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    }
}
