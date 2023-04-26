using System;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using EveSquadron.Models;
using EveSquadron.Models.EVE.Data;
using EveSquadron.Models.EveSquadron;
using EveSquadron.ViewModels;
using EveSquadron.Views.Converters;

namespace EveSquadron.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    override protected void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is not MainWindowViewModel mainWindowViewModel)
            return;

        Title = AppConstants.ApplicationName;
        
        var isDarkModeBinding = new Binding
        {
            Source = mainWindowViewModel,
            Path = nameof(mainWindowViewModel.ThemeVariant)
        };

        var isDarkModeCheckedBinding = new Binding
        {
            Source = mainWindowViewModel,
            Path = nameof(mainWindowViewModel.ThemeVariant),
            Converter = new BooleanToDarkThemeVariantConverter()
        };

        IsDarkMode.Bind(ToggleButton.IsCheckedProperty, isDarkModeCheckedBinding);
        Application.Current!.Bind(Application.RequestedThemeVariantProperty, isDarkModeBinding);

        var alwaysOnTopBinding = new Binding
        {
            Source = mainWindowViewModel,
            Path = nameof(mainWindowViewModel.AlwaysOnTop)
        };
        this.Bind(TopmostProperty, alwaysOnTopBinding);
    }
    
    private void OnFetchErrorClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        button.IsVisible = false;
    }

    private void PlayerInfoGrid_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not DataGrid {DataContext: MainWindowViewModel mainWindowViewModel, CurrentColumn.Header: string clickedColumn } ||
            e.Source is not ILogical logical ||
            logical.GetLogicalParent<DataGridCell>()?.DataContext is not EveSquadronPlayerInformation playerInformation) return;
        
       mainWindowViewModel.OpenZKillboardLinkFor(playerInformation, clickedColumn);
    }
}