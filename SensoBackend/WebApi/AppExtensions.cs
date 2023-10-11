using Asp.Versioning.ApiExplorer;
using SensoBackend.WebApi.Middlewares;

namespace SensoBackend.WebApi;

public static class AppExtensions
{
    public static void UseWebApiLayer(this WebApplication app)
    {
        var apiVersionDescriptionProvider =
            app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (
                    var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse()
                )
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName
                    );
                }
            });
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.UseMiddleware<ExceptionMiddleware>();
    }
}
