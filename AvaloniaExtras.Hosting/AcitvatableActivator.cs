using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaExtras.Hosting;

public static class AcitvatableActivator
{
    public static void Bind(this IActivatable? activatable, Control? control)
    {
        ArgumentNullException.ThrowIfNull(activatable);
        ArgumentNullException.ThrowIfNull(control);

        control.Loaded += Loaded;
        control.Unloaded += Unloaded;

        return;

        void Loaded(object? sender, RoutedEventArgs e)
        {
            activatable.Activate();
        }

        void Unloaded(object? sender, RoutedEventArgs e)
        {
            activatable.Deactivate();

            control.Loaded -= Loaded;
            control.Unloaded -= Unloaded;
        }
    }
}
