using Avalonia.Controls;

namespace AvaloniaExtras.Controls.SpacedGrid;

/// <summary>
///
/// </summary>
public class SpacingColumnDefinition : ColumnDefinition, ISpacingDefinition
{
    /// <inheritdoc />
    public double Spacing
    {
        get => Width.Value;
        set => Width = new GridLength(value, GridUnitType.Pixel);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="width"></param>
    public SpacingColumnDefinition(double width)
        : base(width, GridUnitType.Pixel) { }
}
