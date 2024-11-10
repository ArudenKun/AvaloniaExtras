using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaExtras.Hosting.Internals;

internal interface IAvaloniaHostingApplicationInitializer
{
    internal void InitializeHost(
        Action<IServiceCollection>? configureServices,
        Action<IServiceProvider>? resolver
    );
}
