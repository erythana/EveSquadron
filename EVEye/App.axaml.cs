using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using EVEye.Extensions;
using EVEye.ViewModels;
using EVEye.Views;
using Microsoft.Extensions.DependencyInjection;

namespace EVEye;

public partial class App : Application
{
    private readonly IServiceProvider _serviceCollection;

    public App(IServiceProvider serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }
    
    public override void Initialize()
    {
        Resources[typeof(IServiceProvider)] = _serviceCollection;
        DataTemplates.Add(_serviceCollection.GetRequiredService<ViewLocator>());
        
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ExpressionObserver.DataValidators.RemoveAll(x => x is DataAnnotationsValidationPlugin);
            desktop.MainWindow = new MainWindow
            {
                DataContext = this.CreateInstance<MainWindowViewModel>()
            };
        }
        else
            throw new PlatformNotSupportedException("This application is only supported on Desktop-Environments.");
        
        base.OnFrameworkInitializationCompleted();
    }
}