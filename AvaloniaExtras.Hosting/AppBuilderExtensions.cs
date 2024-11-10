using Avalonia;
using AvaloniaExtras.Hosting.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaExtras.Hosting;

/// <summary>
///
/// </summary>
public static class AppBuilderExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="appBuilder"></param>
    /// <param name="configureServices"></param>
    /// <param name="afterStart"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
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
