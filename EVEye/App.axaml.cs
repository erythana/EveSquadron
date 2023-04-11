using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EVEye.Extensions;
using EVEye.ViewModels;
using EVEye.Views;
using Microsoft.Extensions.DependencyInjection;

namespace EVEye;

public class App : Application
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