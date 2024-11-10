using System;
using System.Collections.Generic;
using System.Globalization;
using AutoInterfaceAttributes;
using JetBrains.Annotations;

namespace AvaloniaExtras.Localization.Abstractions;

/// <summary>
///
/// </summary>
[AutoInterface(Name = "ILocalizer")]
[PublicAPI]
public abstract class BaseLocalizer : ILocalizer
{
    /// <summary>
    ///
    /// </summary>
    public CultureInfo DefaultLanguage { get; set; } = new("en-US");

    private readonly List<CultureInfo> _languages = [];

    /// <summary>
    ///
    /// </summary>
    public List<CultureInfo> Languages
    {
        get
        {
            if (!HasLoaded)
                Reload();

            return _languages;
        }
    }

    private CultureInfo _language = CultureInfo.CurrentCulture;

    /// <summary>
    ///
    /// </summary>
    public CultureInfo Language
    {
        get => _language;
        set => SetLanguage(value);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="language"></param>
    protected virtual void SetLanguage(CultureInfo language) => _language = language;

    /// <summary>
    ///
    /// </summary>
    protected bool HasLoaded { get; set; }

    /// <summary>
    ///
    /// </summary>
    public abstract void Reload();

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
#pragma warning disable CA1716
    public abstract string Get(string key);
#pragma warning restore CA1716

    /// <summary>
    ///
    /// </summary>
    public event EventHandler? LanguageChanged;

    /// <summary>
    ///
    /// </summary>
    protected void RefreshUi()
    {
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }
}
