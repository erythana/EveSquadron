using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace EveSquadron.Models.Helper;

public static class ApplicationHelper
{
    public static Window? GetMainWindow(object? dataContext = null)
    {
        return Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime {MainWindow: {} mainWindow} applicationLifetime && (dataContext is null || mainWindow.DataContext?.GetType() == dataContext?.GetType())
            ? mainWindow
            : null;
    }

    public static Window? GetWindowWithDataContext(object? dataContext)
    {
        return Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime applicationLifetime &&
               applicationLifetime.Windows.FirstOrDefault(x => x.DataContext == dataContext) is { } window
            ? window
            : null;
    }
}