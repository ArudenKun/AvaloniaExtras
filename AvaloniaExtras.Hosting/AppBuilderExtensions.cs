using Avalonia;
using AvaloniaExtras.Hosting.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaExtras.Hosting;

public static class AppBuilderExtensions
{
    public static AppBuilder UseHosting(
        this AppBuilder appBuilder,
        Action<IServiceCollection>? configureServices = null,
        Action<IServiceProvider>? afterStart = null
    )
    {
        appBuilder.AfterSetup(_ =>
        {
            if (
                appBuilder.Instance
                // ReSharper disable once SuspiciousTypeConversion.Global
                is IAvaloniaHostingApplicationInitializer avaloniaHostingApplicationInitializer
            )
            {
                avaloniaHostingApplicationInitializer.InitializeHost(configureServices, afterStart);
            }
            else
            {
                throw new NotSupportedException(
                    "Application must inherit from AvaloniaHostingApplication."
                );
            }
        });
        return appBuilder;
    }
}
