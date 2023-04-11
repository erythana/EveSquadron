using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using EVEye.Models;
using EVEye.ViewModels;
using EVEye.Views.Converters;

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
        Application.Current.Bind(Application.RequestedThemeVariantProperty, isDarkModeBinding);

        var alwaysOnTopBinding = new Binding
        {
            Source = mainWindowViewModel,
            Path = nameof(mainWindowViewModel.AlwaysOnTop)
        };
        this.Bind(TopmostProperty, alwaysOnTopBinding);
    }
}