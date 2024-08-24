using Avalonia.Controls;

namespace AvaloniaExtras.Controls.SpacedGrid;

public class SpacingColumnDefinition : ColumnDefinition, ISpacingDefinition
{
    public double Spacing
    {
        get => Width.Value;
        set => Width = new GridLength(value, GridUnitType.Pixel);
    }

    public SpacingColumnDefinition(double width)
        : base(width, GridUnitType.Pixel) { }
}
