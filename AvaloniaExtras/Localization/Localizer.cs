using System.Globalization;
using JetBrains.Annotations;

namespace AvaloniaExtras.Localization;

// https://github.com/tifish/Jeek.Avalonia.Localization
/// <summary>
///
/// </summary>
[PublicAPI]
public static class Localizer
{
    private static ILocalizer? _localizer;

    private static ILocalizer Current =>
        _localizer ?? throw new InvalidOperationException("Localizer was is not set");

    /// <summary>
    ///
    /// </summary>
    /// <param name="localizer"></param>
    public static void SetLocalizer(ILocalizer localizer) => _localizer = localizer;

    /// <summary>
    ///
    /// </summary>
#pragma warning disable CA1002
    public static List<CultureInfo> Languages => Current.Languages;
#pragma warning restore CA1002

    /// <summary>
    ///
    /// </summary>
    public static CultureInfo Language
    {
        get => Current.Language;
        set => Current.Language = value;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string Get(string key) => Current.Get(key);

    /// <summary>
    ///
    /// </summary>
    public static event EventHandler? LanguageChanged
    {
        add => Current.LanguageChanged += value;
        remove => Current.LanguageChanged -= value;
    }
}
