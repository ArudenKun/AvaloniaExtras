using Avalonia.Controls;

namespace AvaloniaExtras.Controls.SpacedGrid;

/// <summary>
///
/// </summary>
public class SpacingRowDefinition : RowDefinition, ISpacingDefinition
{
    /// <inheritdoc />
    public double Spacing
    {
        get => Height.Value;
        set => Height = new GridLength(value, GridUnitType.Pixel);
    }

    /// <inheritdoc />
    public SpacingRowDefinition(double height)
        : base(height, GridUnitType.Pixel) { }
}
