using System;
using System.IO;
using Avalonia;
using Avalonia.ReactiveUI;
using EveSquadron.DependencyInjection;
using EveSquadron.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EveSquadron;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        
        var services = BuildServices();
        var startupLogger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            if (!Directory.Exists(AppConstants.ConfigurationDirectory))
                Directory.CreateDirectory(AppConstants.ConfigurationDirectory);
            
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            startupLogger.LogCritical(e, $"Unhandled exception in {AppConstants.ApplicationName} - Application shutdown.");
        }

    }

    private static ServiceProvider BuildServices() =>
        new ServiceCollection()
            .Configure()
            .BuildServiceProvider();

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure(() => new App(BuildServices()))
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}