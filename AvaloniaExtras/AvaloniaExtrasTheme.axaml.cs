using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace AvaloniaExtras;

/// <summary>
///
/// </summary>
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
