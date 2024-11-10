using System;
using System.Collections.Generic;
using System.Globalization;
using AvaloniaExtras.Localization.Abstractions;
using AvaloniaExtras.Localization.Localizers;
using JetBrains.Annotations;

namespace AvaloniaExtras.Localization;

/// <summary>
///
/// </summary>
[PublicAPI]
public static class Localizer
{
    private static ILocalizer _localizer = new JsonLocalizer();

    /// <summary>
    ///
    /// </summary>
    /// <param name="localizer"></param>
    public static void SetLocalizer(ILocalizer localizer) => _localizer = localizer;

    /// <summary>
    ///
    /// </summary>
#pragma warning disable CA1002
    public static List<CultureInfo> Languages => _localizer.Languages;
#pragma warning restore CA1002

    /// <summary>
    ///
    /// </summary>
    public static CultureInfo Language
    {
        get => _localizer.Language;
        set => _localizer.Language = value;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string Get(string key) => _localizer.Get(key);

    /// <summary>
    ///
    /// </summary>
    public static event EventHandler? LanguageChanged
    {
        add => _localizer.LanguageChanged += value;
        remove => _localizer.LanguageChanged -= value;
    }
}
