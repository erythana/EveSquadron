﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.Models;
using EveSquadron.Models.EveSquadron;
using EveSquadron.Models.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace EveSquadron.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region member fields

    private readonly IPlayerInformationDataAggregator _playerInformationDataAggregator;
    private readonly IReleaseVersionChecker _releaseVersionChecker;
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly DispatcherTimer _clipboardPollingTimer;
    private string? _previousClipboardContent = string.Empty;

    private bool _alwaysOnTop;
    private ThemeVariant _themeVariant;
    private Task<bool?> _updateAvailable;

    #endregion

    #region constructor

    public MainWindowViewModel(IPlayerInformationDataAggregator playerInformationDataAggregator, IReleaseVersionChecker releaseVersionChecker, IAppSettingsLoader settings, ILogger<MainWindowViewModel> logger)
    {
        _themeVariant = ThemeVariant.Default;
        _updateAvailable = Task.FromResult<bool?>(false);
        _playerInformationDataAggregator = playerInformationDataAggregator;
        _releaseVersionChecker = releaseVersionChecker;
        _logger = logger;
        
        var version = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location).ProductVersion ?? "";
        if(_releaseVersionChecker.TryParseVersionNumber(version, out var recognizedVersion))
            UpdateAvailable = _releaseVersionChecker.IsNewReleaseAvailable(recognizedVersion.major, recognizedVersion.minor, recognizedVersion.patch);

        UpdateClickedCommand = ReactiveCommand.Create(OnUpdateButtonClicked);
        EveSquadronPlayerInformation = new ObservableCollection<EveSquadronPlayerInformation>();
        ThemeVariant = settings.Theme switch
        {
            { } theme when theme.Equals("Dark", StringComparison.InvariantCultureIgnoreCase) => ThemeVariant.Dark,
            { } theme when theme.Equals("Light", StringComparison.InvariantCultureIgnoreCase) => ThemeVariant.Light,
            _ => ThemeVariant.Default
        };

        var configuredPollingRate = settings.ClipboardPollingMilliseconds;
        var timerTickRate = TryParseClipboardPollingRate(configuredPollingRate);

        _clipboardPollingTimer = new DispatcherTimer(timerTickRate, DispatcherPriority.Background, ClipboardPollingTimerCallback)
        {
            IsEnabled = true
        };
    }

    #endregion

    #region event handler

    private async void ClipboardPollingTimerCallback(object? sender, EventArgs e)
    {
        var clipboardContent = await Application.Current!.Clipboard!.GetTextAsync();
        if (clipboardContent == _previousClipboardContent || clipboardContent is null)
            return;
        _previousClipboardContent = clipboardContent;

        //TODO: MAYBE PAUSE TIMER WHILE DOING THE REST
        await TryParseClipboardContentForEve(clipboardContent);
    }

    #endregion

    #region properties

    public bool AlwaysOnTop {
        get => _alwaysOnTop;
        set => SetProperty(ref _alwaysOnTop, value);
    }
    
    public ReactiveCommand<Unit, Unit> UpdateClickedCommand { get; }

    public Task<bool?> UpdateAvailable {
        get => _updateAvailable;
        private set => SetProperty(ref _updateAvailable, value);
    }

    public string LatestReleasePath {
        get => _releaseVersionChecker.ReleasePath;
    }

    public ThemeVariant ThemeVariant {
        get => _themeVariant;
        set => SetProperty(ref _themeVariant, value);
    }


    // ReSharper disable once InconsistentNaming
    public ObservableCollection<EveSquadronPlayerInformation> EveSquadronPlayerInformation { get; set; }

    #endregion

    #region helper methods

    private static TimeSpan TryParseClipboardPollingRate(string? configuredPollingRate)
    {
        if (double.TryParse(configuredPollingRate, out var pollingRate))
            pollingRate = Math.Clamp(pollingRate, 100, 1000);
        else
            pollingRate = 250; //Use 250ms as fallback

        var timerTickRate = TimeSpan.FromMilliseconds(pollingRate);

        return timerTickRate;
    }

    private async Task TryParseClipboardContentForEve(string clipboardContent)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(clipboardContent))
                return;

            _logger.LogDebug("New Clipboard-Copy detected. Trying to parse player names");
            
            EveSquadronPlayerInformation.Clear();

            var clipboardSplit = Regex.Split(clipboardContent, "\r?\n")//This replaces Environment.NewLine and splits the newlines  because, for some reason, Windows used \n on a users system
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct()
                .ToList();

            var nameCharacterLimit = ApplicationConstants.EveAPILimits.PostUniverseIDsNameCharacterLimit;
            if (clipboardSplit.Any(x => x.Length > nameCharacterLimit))
            {
                _logger.LogDebug($"TryParseClipboardContentForEve: Could not load player information. These arent proper eve names, the limit on characters in a name is {nameCharacterLimit}");
                return;
            }
            
            await foreach (var eveSquadronPlayerInformation in _playerInformationDataAggregator.GetAggregatedItemsFor(clipboardSplit))
            {
                EveSquadronPlayerInformation.Add(eveSquadronPlayerInformation);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not load player information. Check input from clipboard!.");
        }
    }
    
    private void OnUpdateButtonClicked() => 
        Process.Start(new ProcessStartInfo(LatestReleasePath) { UseShellExecute = true });

    #endregion
}