using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace AvaloniaExtras;

/// <summary>
///
/// </summary>
public class AvaloniaExtrasTheme : Styles
{
    /// <inheritdoc />
    public AvaloniaExtrasTheme()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
