using Avalonia.Controls;
using Avalonia.Interactivity;
using EveSquadron.ViewModels.Interfaces;

namespace EveSquadron.Views;

public partial class WhitelistManagementView : Window
{
    public WhitelistManagementView()
    {
        InitializeComponent();
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (DataContext is IWhitelistManagementViewModel whitelistManagementViewModel)
            whitelistManagementViewModel.IsWindowVisible = false;
    }
    
    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IWhitelistManagementViewModel whitelistManagementViewModel)
            whitelistManagementViewModel.IsWindowVisible = true;
    }
}