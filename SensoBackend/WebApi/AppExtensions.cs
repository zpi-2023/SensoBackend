using SensoBackend.WebApi.Middlewares;

namespace SensoBackend.WebApi;

public static class AppExtensions
{
    public static void UseWebApiLayer(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
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
