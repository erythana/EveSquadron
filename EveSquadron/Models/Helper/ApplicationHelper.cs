using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace EveSquadron.Models.Helper;

public static class ApplicationHelper
{
    public static Window? GetMainWindow()
    {
        return Application.Current!.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime {MainWindow: {} mainWindow}
            ? null
            : mainWindow;

    }
}