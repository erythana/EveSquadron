using System;
using System.Diagnostics;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using EveSquadron.Models.Interfaces;
using EveSquadron.Models.Options;
using EveSquadron.ViewModels.Interfaces;
using EveSquadron.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace EveSquadron.ViewModels;

public class StatusBarViewModel : ViewModelBase, IStatusBarViewModel
{
    #region member fields

    private readonly IReleaseVersionChecker _releaseVersionChecker;
    private readonly ILogger<IStatusBarViewModel> _logger;
    private bool _alwaysOnTop;
    private Task<bool?> _updateAvailable;
    private ThemeVariant _themeVariant;
    private bool _whitelistActive;

    #endregion

    #region constructor

    public StatusBarViewModel(IOptions<ThemeOptions> themeOptions, IReleaseVersionChecker releaseVersionChecker, ILogger<IStatusBarViewModel> logger)
    {
        _releaseVersionChecker = releaseVersionChecker;
        _logger = logger;
        _updateAvailable = Task.FromResult<bool?>(false);
        _themeVariant = ThemeVariant.Default;
        
        var version = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location).ProductVersion ?? "";
        if (_releaseVersionChecker.TryParseVersionNumber(version, out var recognizedVersion))
            UpdateAvailable = _releaseVersionChecker.IsNewReleaseAvailable(recognizedVersion.major, recognizedVersion.minor, recognizedVersion.patch);
        
        OpenUpdateCommand = ReactiveCommand.Create(OnOpenUpdateCommand);
        OpenWhitelistCommand = ReactiveCommand.CreateFromTask(OnOpenWhitelistCommand);
        OpenSettingsCommand = ReactiveCommand.CreateFromTask(OnOpenSettingsCommand);
        
        ThemeVariant = themeOptions.Value.Theme switch
        {
            { } theme when theme.Equals("Dark", StringComparison.InvariantCultureIgnoreCase) => ThemeVariant.Dark,
            { } theme when theme.Equals("Light", StringComparison.InvariantCultureIgnoreCase) => ThemeVariant.Light,
            _ => ThemeVariant.Default
        };
    }
    
    #endregion

    #region properties
    
    public ThemeVariant ThemeVariant {
        get => _themeVariant;
        set => SetProperty(ref _themeVariant, value);
    }

    public bool WhitelistActive {
        get => _whitelistActive;
        set => SetProperty(ref _whitelistActive, value);
    }

    public bool AlwaysOnTop {
        get => _alwaysOnTop;
        set => SetProperty(ref _alwaysOnTop, value);
    }

    public ReactiveCommand<Unit, Unit> OpenUpdateCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenWhitelistCommand { get; }
    
    public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }

    public Task<bool?> UpdateAvailable {
        get => _updateAvailable;
        private set => SetProperty(ref _updateAvailable, value);
    }

    public string LatestReleasePath {
        get => _releaseVersionChecker.ReleasePath;
    }
    
    #endregion

    #region helper methods
    
    private void OnOpenUpdateCommand() =>
        Process.Start(new ProcessStartInfo(LatestReleasePath)
        {
            UseShellExecute = true
        });
    
    private async Task OnOpenWhitelistCommand()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime applicationLifetime)
            return;

        try  //ReactiveCommand
        {
            var dialog = new WhitelistManagementView();
            
            if (Application.Current.DataContext is IMainWindowViewModel mainWindowViewModel)
                dialog.DataContext = mainWindowViewModel.WhitelistManagementViewModel;
            await dialog.ShowDialog<IWhitelistManagementViewModel?>(applicationLifetime.MainWindow!);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Can't open WhitelistManagementView");
            throw;
        }
        finally
        {
            applicationLifetime.MainWindow!.IsEnabled = true;
        }
    }
    
    private async Task OnOpenSettingsCommand()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime applicationLifetime)
            return;

        try  //ReactiveCommand
        {
            var dialog = new SettingsManagementView();
            
            if (Application.Current.DataContext is IMainWindowViewModel mainWindowViewModel)
                dialog.DataContext = mainWindowViewModel.SettingsManagementViewModel;
            await dialog.ShowDialog<ISettingsManagementViewModel?>(applicationLifetime.MainWindow!);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Can't open SettingsManagementView");
            throw;
        }
        finally
        {
            applicationLifetime.MainWindow!.IsEnabled = true;
        }
    }
    
    #endregion
}