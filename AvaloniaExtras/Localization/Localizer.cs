using System;
using System.Collections.Generic;
using System.Globalization;
using AvaloniaExtras.Localization.Abstractions;
using JetBrains.Annotations;

namespace AvaloniaExtras.Localization;

/// <summary>
///
/// </summary>
[PublicAPI]
public static class Localizer
{
    private static ILocalizer? _localizer;

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static ILocalizer CurrentLocalizer =>
        _localizer ?? throw new InvalidOperationException("Localizer was is set");

    /// <summary>
    ///
    /// </summary>
    /// <param name="localizer"></param>
    public static void SetLocalizer(ILocalizer localizer) => _localizer = localizer;

    /// <summary>
    ///
    /// </summary>
#pragma warning disable CA1002
    public static List<CultureInfo> Languages => CurrentLocalizer.Languages;
#pragma warning restore CA1002

    /// <summary>
    ///
    /// </summary>
    public static CultureInfo Language
    {
        get => CurrentLocalizer.Language;
        set => CurrentLocalizer.Language = value;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string Get(string key) => CurrentLocalizer.Get(key);

    /// <summary>
    ///
    /// </summary>
    public static event EventHandler? LanguageChanged
    {
        add => CurrentLocalizer.LanguageChanged += value;
        remove => CurrentLocalizer.LanguageChanged -= value;
    }
}
