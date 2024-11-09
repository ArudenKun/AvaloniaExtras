using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using AvaloniaExtras.Localization.Abstractions;
using JetBrains.Annotations;

namespace AvaloniaExtras.Localization.Localizers;

/// <summary>
///
/// </summary>
[PublicAPI]
public sealed partial class JsonLocalizer : BaseLocalizer
{
    private readonly string _languageJsonDirectory;

    private Dictionary<string, string> _languageStrings = new();

    /// <summary>
    ///
    /// </summary>
    /// <param name="languageJsonDirectory"></param>
    public JsonLocalizer(string? languageJsonDirectory = null)
    {
        _languageJsonDirectory =
            languageJsonDirectory
            ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Translations");
    }

    /// <inheritdoc />
    public override void Reload()
    {
        _languageStrings.Clear();
        Languages.Clear();

        if (!Directory.Exists(_languageJsonDirectory))
            throw new FileNotFoundException(_languageJsonDirectory);

        foreach (var file in Directory.GetFiles(_languageJsonDirectory, "*.json"))
        {
            var language = Path.GetFileNameWithoutExtension(file);
            var match = LanguageRegex().Match(language);
            if (match.Success)
            {
                Languages.Add(new CultureInfo(language));
            }
        }

        if (!Languages.Contains(Language))
            Language = DefaultLanguage;

        var languageFile = Path.Combine(_languageJsonDirectory, Language + ".json");
        if (!File.Exists(languageFile))
            throw new FileNotFoundException($"No language file ${languageFile}");

        using var jsonStream = File.OpenRead(languageFile);
        using var doc = JsonDocument.Parse(
            jsonStream,
            new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
            }
        );

        foreach (var element in doc.RootElement.EnumerateObject())
        {
            _languageStrings.Add(
                element.Name.ToLower(),
                element.Value.GetString() ?? $"{element.Name} is null"
            );
        }

        HasLoaded = true;
    }

    /// <inheritdoc />
    protected override void SetLanguage(CultureInfo language)
    {
        base.SetLanguage(language);
        Reload();
        RefreshUi();
    }

    /// <inheritdoc />
    public override string Get(string key)
    {
        if (!HasLoaded)
            Reload();

        if (_languageStrings == null)
            throw new Exception("No language strings loaded.");

        return _languageStrings.TryGetValue(key.ToLower(), out var langStr)
            ? langStr.Replace("\\n", "\n")
            : $"{Language}:{key}";
    }

    [GeneratedRegex(@"^\w+\.[a-z]{2}(?:-[A-Z]{2})?$", RegexOptions.Compiled)]
    private partial Regex LanguageRegex();
}
