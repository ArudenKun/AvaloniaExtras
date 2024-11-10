using Avalonia.Controls;
using DependencyPropertyGenerator;
using JetBrains.Annotations;

namespace AvaloniaExtras.Controls;

/// <summary>
///
/// </summary>
[DependencyProperty<bool>("Condition")]
[DependencyProperty<Control>("TrueContent")]
[DependencyProperty<Control>("FalseContent")]
[PublicAPI]
#pragma warning disable CA1716
public partial class If : UserControl
#pragma warning restore CA1716
{
    /// <summary>
    ///
    /// </summary>
    public If()
    {
        InitializeComponent();
    }

    partial void OnConditionChanged() => UpdateContent();

    partial void OnTrueContentChanged() => UpdateContent();

    partial void OnFalseContentChanged() => UpdateContent();

    private void UpdateContent()
    {
        Content = Condition ? TrueContent : FalseContent;
    }
}
