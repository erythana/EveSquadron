using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using EVEye.Models;
using EVEye.ViewModels;

namespace EVEye.Views;

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

        Title = ApplicationConstants.ApplicationName;
        var alwaysOnTopBinding = new Binding//Bind here because the data context is not yet set within the axaml
        {
            Source = mainWindowViewModel,
            Path = nameof(mainWindowViewModel.AlwaysOnTop)
        };
        this.Bind(TopmostProperty, alwaysOnTopBinding);
    }
}