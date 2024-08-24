using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace AvaloniaExtras.Controls.SpacedGrid;

/// <summary>
///
/// </summary>
public class SpacedGrid : Grid
{
    #region Properties

    /// <summary>
    ///
    /// </summary>
    public static readonly StyledProperty<double> RowSpacingProperty = AvaloniaProperty.Register<
        SpacedGrid,
        double
    >(nameof(RowSpacing), 3);

    /// <summary>
    ///
    /// </summary>
    public static readonly StyledProperty<double> ColumnSpacingProperty = AvaloniaProperty.Register<
        SpacedGrid,
        double
    >(nameof(ColumnSpacing), 3);

    /// <summary>
    ///
    /// </summary>
    public double RowSpacing
    {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    /// <summary>
    ///
    /// </summary>
    public double ColumnSpacing
    {
        get => GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    /// <summary>
    /// Returns an enumerable of all the grid's row definitions, <u>excluding</u> spacing rows.
    /// </summary>
    public IEnumerable<RowDefinition> UserDefinedRowDefinitions =>
        RowDefinitions.Where(definition => definition is not ISpacingDefinition);

    /// <summary>
    /// Returns an enumerable of all the grid's column definitions, <u>excluding</u> spacing columns.
    /// </summary>
    public IEnumerable<ColumnDefinition> UserDefinedColumnDefinitions =>
        ColumnDefinitions.Where(definition => definition is not ISpacingDefinition);

    #endregion Properties

    #region Construction

    /// <summary>
    ///
    /// </summary>
    public SpacedGrid() => Children.CollectionChanged += Children_CollectionChanged;

    #endregion Construction

    #region Override methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

        RowDefinitions.CollectionChanged += delegate
        {
            UpdateSpacedRows();
        };
        ColumnDefinitions.CollectionChanged += delegate
        {
            UpdateSpacedColumns();
        };

        UpdateSpacedRows();
        UpdateSpacedColumns();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        switch (change.Property.Name)
        {
            case nameof(RowSpacing):
                RecalculateRowSpacing();
                break;

            case nameof(ColumnSpacing):
                RecalculateColumnSpacing();
                break;
        }
    }

    #endregion Override methods

    #region Events

    private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (
            e.Action
            is not (NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace)
        )
            return;

        if (e.NewItems is not { } newItems)
        {
            return;
        }

        foreach (Control item in newItems)
            item.Initialized += Item_Initialized;
    }

    private void Item_Initialized(object? sender, System.EventArgs e)
    {
        if (sender is not Control item)
        {
            return;
        }

        item.Initialized -= Item_Initialized;

        SetRow(item, GetRow(item) * 2); // 1 -> 2 or 2 -> 4
        SetRowSpan(item, (GetRowSpan(item) * 2) - 1); // 2 -> 3 or 3 -> 5

        SetColumn(item, GetColumn(item) * 2); // 1 -> 2 or 2 -> 4
        SetColumnSpan(item, (GetColumnSpan(item) * 2) - 1); // 2 -> 3 or 3 -> 5
    }

    #endregion Events

    #region Other methods

    private void UpdateSpacedRows()
    {
        var userRowDefinitions = UserDefinedRowDefinitions.ToList(); // User-defined rows (e.g. the ones defined in XAML files)
        var actualRowDefinitions = new RowDefinitions(); // User-defined + spacing rows

        int currentUserDefinition = 0,
            currentActualDefinition = 0;

        while (currentUserDefinition < userRowDefinitions.Count)
        {
            if (currentActualDefinition % 2 == 0) // Even rows are user-defined rows (0, 2, 4, 6, 8, 10, ...)
            {
                actualRowDefinitions.Add(userRowDefinitions[currentUserDefinition]);
                currentUserDefinition++;
            }
            else // Odd rows are spacing rows (1, 3, 5, 7, 9, 11, ...)
                actualRowDefinitions.Add(new SpacingRowDefinition(RowSpacing));

            currentActualDefinition++;
        }

        RowDefinitions = actualRowDefinitions;
        RowDefinitions.CollectionChanged += delegate
        {
            UpdateSpacedRows();
        };
    }

    private void UpdateSpacedColumns()
    {
        var userColumnDefinitions = UserDefinedColumnDefinitions.ToList(); // User-defined columns (e.g. the ones defined in XAML files)
        var actualColumnDefinitions = new ColumnDefinitions(); // User-defined + spacing columns

        int currentUserDefinition = 0,
            currentActualDefinition = 0;

        while (currentUserDefinition < userColumnDefinitions.Count)
        {
            if (currentActualDefinition % 2 == 0) // Even columns are user-defined columns (0, 2, 4, 6, 8, 10, ...)
            {
                actualColumnDefinitions.Add(userColumnDefinitions[currentUserDefinition]);
                currentUserDefinition++;
            }
            else // Odd columns are spacing columns (1, 3, 5, 7, 9, 11, ...)
                actualColumnDefinitions.Add(new SpacingColumnDefinition(ColumnSpacing));

            currentActualDefinition++;
        }

        ColumnDefinitions = actualColumnDefinitions;
        ColumnDefinitions.CollectionChanged += delegate
        {
            UpdateSpacedColumns();
        };
    }

    private void RecalculateRowSpacing()
    {
        foreach (ISpacingDefinition spacingRow in RowDefinitions.OfType<ISpacingDefinition>())
            spacingRow.Spacing = RowSpacing;
    }

    private void RecalculateColumnSpacing()
    {
        foreach (ISpacingDefinition spacingColumn in ColumnDefinitions.OfType<ISpacingDefinition>())
            spacingColumn.Spacing = ColumnSpacing;
    }

    #endregion Other methods
}
