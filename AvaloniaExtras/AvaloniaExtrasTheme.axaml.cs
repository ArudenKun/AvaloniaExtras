using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using JetBrains.Annotations;

namespace AvaloniaExtras;

/// <summary>
///
/// </summary>
[PublicAPI]
public class AvaloniaExtrasTheme : Styles
{
    /// <summary>
    ///
    /// </summary>
    public AvaloniaExtrasTheme()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
