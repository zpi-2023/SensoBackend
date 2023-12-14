using System.Reflection;
using FluentValidation;
using Mapster;
using MediatR;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Behaviors;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Application.PushNotifications;
using SensoBackend.Application.RemindersScheduler;

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

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<ISeniorIdRepo, SeniorIdRepo>();

        services.AddTransient<IPayloadFactory, PayloadFactory>();
        services.AddTransient<IAlertDispatcher, AlertDispatcher>();
        services.AddSingleton<IHangfireWrapper, HangfireWrapper>();

        services.AddHostedService<InitializeExistingReminders>();

        TypeAdapterConfig.GlobalSettings.Scan(Assembly);
    }
}
