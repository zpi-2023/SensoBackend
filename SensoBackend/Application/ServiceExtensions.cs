using FluentValidation;
using Mapster;
using MediatR;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Behaviors;
using System.Reflection;

namespace SensoBackend.Application;

public static class ServiceExtensions
{
    private static readonly Assembly Assembly = typeof(ServiceExtensions).Assembly;

    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly));
        services.AddValidatorsFromAssembly(Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddSingleton<ITimeProvider, TimeProvider>();

        TypeAdapterConfig.GlobalSettings.Scan(Assembly);
    }
}
