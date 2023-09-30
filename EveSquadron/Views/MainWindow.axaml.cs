using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using EveSquadron.Models;
using EveSquadron.Models.EveSquadron;
using EveSquadron.ViewModels.Interfaces;

namespace EveSquadron.Views;

public partial class MainWindow : Window
{
    #region member fields

    private readonly List<DataGridRow> _visibleDataGridRows;
    private Color _hoverColor = Colors.Red; //Should not happen without setting it explicitly in config, but just to provide an high contrast as fallback

    #endregion

    #region constructor

    public MainWindow()
    {
        InitializeComponent();

        _visibleDataGridRows = new List<DataGridRow>();

        PointerEnteredEvent.AddClassHandler<DataGridRow>(SetHoverBackgroundForPlayerGroup);
        PointerExitedEvent.AddClassHandler<DataGridRow>(RemoveHoverBackground);
    }

    #endregion

    #region overrides

    override protected void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is not IMainWindowViewModel currentViewModel)
            return;

        Title = AppConstants.ApplicationName;

        _hoverColor = currentViewModel.HoverColor;
        
        var alwaysOnTopBinding = new Binding
        {
            Source = currentViewModel.StatusBarViewModel,
            Path = nameof(currentViewModel.StatusBarViewModel.AlwaysOnTop),
        };
        
        var isDarkModeCheckedBinding = new Binding
        {
            Source = currentViewModel.StatusBarViewModel,
            Path = nameof(currentViewModel.StatusBarViewModel.ThemeVariant),
            Mode = BindingMode.OneWay
        };
        
        this.Bind(TopmostProperty, alwaysOnTopBinding);
        Application.Current!.Bind(Application.RequestedThemeVariantProperty, isDarkModeCheckedBinding);
    }

    #endregion

    #region event handler

    private void OnFetchErrorClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        button.IsVisible = false;
    }

    private void PlayerInfoGrid_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not DataGrid { DataContext: IMainWindowViewModel mainWindowViewModel, CurrentColumn.Header: string clickedColumn } ||
            e.Source is not ILogical logical ||
            logical.GetLogicalParent<DataGridCell>()?.DataContext is not EveSquadronPlayerInformation playerInformation ||
            !Enum.TryParse(clickedColumn, out EntityTypeEnum target)) return;
        
        mainWindowViewModel.OpenZKillboardLinkFor(playerInformation, target);
    }

    private void PlayerInfoGrid_OnCopyingRowClipboardContent(object? sender, DataGridRowClipboardEventArgs e)
    {
        var rowContent = e.ClipboardRowContent.ToList();
        foreach (var cellContent in rowContent)
        {
            if (cellContent.Content is Task<Bitmap>)
                e.ClipboardRowContent.Remove(cellContent); //Exclude Images from copy
        }
    }

    private void RemoveHoverBackground(DataGridRow dataGridRow, PointerEventArgs pointerEventArgs)
    {
        foreach (var gridRow in _visibleDataGridRows)
            gridRow.Background = Brushes.Transparent;
    }

    private void SetHoverBackgroundForPlayerGroup(DataGridRow dataGridRow, PointerEventArgs pointerEventArgs)
    {
        if (dataGridRow.DataContext is not EveSquadronPlayerInformation playerInformation)
            return;

        var corpMember = _visibleDataGridRows.Where(x => x.DataContext is EveSquadronPlayerInformation { Corporation: not null } player &&
                                                         player.Corporation.ID == playerInformation.Corporation?.ID);
        var allianceMember = _visibleDataGridRows.Where(x => x.DataContext is EveSquadronPlayerInformation { Alliance: not null } player &&
                                                             player.Alliance.ID == playerInformation.Alliance?.ID);

        var corpBrush = new SolidColorBrush(_hoverColor, 0.65);
        var allianceBrush = new SolidColorBrush(_hoverColor, 0.4);
        foreach (var gridRow in allianceMember)
            gridRow.Background = allianceBrush;
        foreach (var gridRow in corpMember)
            gridRow.Background = corpBrush;

        dataGridRow.Background = new SolidColorBrush(_hoverColor);
    }
    
    private void PlayerInfoGrid_OnLoadingRow(object? sender, DataGridRowEventArgs e) =>
        _visibleDataGridRows.Add(e.Row);

    private void PlayerInfoGrid_OnUnloadingRow(object? sender, DataGridRowEventArgs e) =>
        _visibleDataGridRows.Remove(e.Row);

    #endregion
}