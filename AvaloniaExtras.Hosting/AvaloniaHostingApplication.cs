using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using AvaloniaExtras.Hosting.Internals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IApplicationLifetime = Avalonia.Controls.ApplicationLifetimes.IApplicationLifetime;

namespace AvaloniaExtras.Hosting;

/// <summary>
///
/// </summary>
/// <typeparam name="TMainWindow"></typeparam>
public abstract class AvaloniaHostingApplication<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMainWindow
> : Application, IAvaloniaHostingApplicationInitializer
    where TMainWindow : Window
{
    private HostApplicationBuilder? _hostBuilder;
    private IHost? _host;

    private Action<IServiceProvider>? _resolver;

    private readonly string _mainWindowKey;

    /// <summary>
    ///
    /// </summary>
    protected AvaloniaHostingApplication() =>
        _mainWindowKey = $"{GetHashCode()}-{typeof(TMainWindow).FullName}";

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public new IClassicDesktopStyleApplicationLifetime ApplicationLifetime =>
        base.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime
        ?? throw new InvalidOperationException(
            "hosting only supports desktop lifetime or lifetime is yet set"
        );

    void IAvaloniaHostingApplicationInitializer.InitializeHost(
        Action<IServiceCollection>? configureServices,
        Action<IServiceProvider>? resolver
    )
    {
        _hostBuilder = Host.CreateApplicationBuilder();
        ConfigureRequiredServices(_hostBuilder.Services);
        ConfigureEnvironment(_hostBuilder.Environment);
        ConfigureServices(_hostBuilder.Services);
        configureServices?.Invoke(_hostBuilder.Services);
        _resolver = resolver;
        ConfigureLogging(_hostBuilder.Logging);
        _host = _hostBuilder.Build();
    }

    private void ConfigureRequiredServices(IServiceCollection services)
    {
        services.AddSingleton<IDispatcher>(_ => Dispatcher.UIThread);
        services.AddSingleton(_ => ApplicationLifetime);
        services.AddSingleton<IApplicationLifetime>(sp =>
            sp.GetRequiredService<IClassicDesktopStyleApplicationLifetime>()
        );
        services.AddKeyedSingleton<TMainWindow>(_mainWindowKey);
        services.AddSingleton<TopLevel>(sp =>
            sp.GetRequiredKeyedService<TMainWindow>(_mainWindowKey)
        );
        services.AddSingleton(sp => sp.GetRequiredService<TopLevel>().StorageProvider);
        services.AddSingleton(sp => sp.GetRequiredService<TopLevel>().Launcher);
        services.AddSingleton(sp => sp.GetRequiredService<TopLevel>().Clipboard!);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="environment"></param>
    protected virtual void ConfigureEnvironment(IHostEnvironment environment) { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="services"></param>
    protected virtual void ConfigureServices(IServiceCollection services) { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    protected virtual void ConfigureLogging(ILoggingBuilder builder) { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="services"></param>
    protected virtual void OnStartup(IServiceProvider services) { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="services"></param>
    protected virtual void OnExit(IServiceProvider services) { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="mainWindow"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    protected virtual Task ConfigureMainWindow(TMainWindow mainWindow, IServiceProvider services) =>
        Task.CompletedTask;

    /// <inheritdoc />
    public sealed override async void OnFrameworkInitializationCompleted()
    {
        if (_host is null)
        {
            throw new InvalidOperationException(
                "The host is not initialized, Please call UseHosting on the AppBuilder first"
            );
        }

        OnStartup(_host.Services);
        _resolver?.Invoke(_host.Services);
        var mainWindow = _host.Services.GetRequiredKeyedService<TMainWindow>(_mainWindowKey);
        await ConfigureMainWindow(mainWindow, _host.Services);
        ApplicationLifetime.MainWindow = mainWindow;
        ApplicationLifetime.Exit += (_, _) =>
        {
            OnExit(_host.Services);
            _host.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
            _host.Dispose();
            _host = null;
        };

        base.OnFrameworkInitializationCompleted();
        await _host.StartAsync();
    }

    /// <inheritdoc />
    public sealed override void RegisterServices() => base.RegisterServices();
}
