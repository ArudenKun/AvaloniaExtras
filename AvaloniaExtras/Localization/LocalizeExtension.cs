using Avalonia;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace AvaloniaExtras.Localization;

// https://github.com/tifish/Jeek.Avalonia.Localization
/// <summary>
///
/// </summary>
/// <param name="key"></param>
[PublicAPI]
public class LocalizeExtension(string key) : MarkupExtension, IObservable<string>, IDisposable
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public override object ProvideValue(IServiceProvider serviceProvider) => this.ToBinding();

    private IObserver<string>? _observer;

    /// <summary>
    ///
    /// </summary>
    /// <param name="observer"></param>
    /// <returns></returns>
    public IDisposable Subscribe(IObserver<string> observer)
    {
        _observer = observer;
        _observer.OnNext(Localizer.Get(key));
        Localizer.LanguageChanged += OnLanguageChanged;

        return this;
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        _observer?.OnNext(Localizer.Get(key));
    }

    /// <inheritdoc cref="IDisposable"/>>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        Localizer.LanguageChanged -= OnLanguageChanged;
        _observer = null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~LocalizeExtension()
    {
        Dispose(false);
    }
}
