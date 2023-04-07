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

    override protected void OnInitialized()
    {
        base.OnInitialized();
        
        if (DataContext is not MainWindowViewModel mainWindowViewModel)
            return;

        Title = ApplicationConstants.ApplicationName;
        //Bind here because the data context is not yet set within the axaml
        var alwaysOnTopBinding = new Binding
        {
            Source = mainWindowViewModel,
            Path = nameof(mainWindowViewModel.AlwaysOnTop)
        };

        this.Bind(TopmostProperty, alwaysOnTopBinding);
    }
}