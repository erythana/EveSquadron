using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Styling;
using Avalonia.Threading;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.Models;
using EveSquadron.Models.EVE.Data;
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
    private readonly Dictionary<int, int> _idCountDictionary;
    private ObservableCollection<EveSquadronPlayerInformation> _eveSquadronPlayerInformation { get; }

    #endregion

    #region constructor

    public MainWindowViewModel(IPlayerInformationDataAggregator playerInformationDataAggregator, IReleaseVersionChecker releaseVersionChecker, IAppSettingsLoader settings, ILogger<MainWindowViewModel> logger)
    {
        _themeVariant = ThemeVariant.Default;
        _updateAvailable = Task.FromResult<bool?>(false);
        _playerInformationDataAggregator = playerInformationDataAggregator;
        _playerInformationDataAggregator.ParsedNewID += OnParsedNewID;
        _idCountDictionary = new Dictionary<int, int>();
        _eveSquadronPlayerInformation = new ObservableCollection<EveSquadronPlayerInformation>();
        _releaseVersionChecker = releaseVersionChecker;
        _logger = logger;

        var version = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location).ProductVersion ?? "";
        if(_releaseVersionChecker.TryParseVersionNumber(version, out var recognizedVersion))
            UpdateAvailable = _releaseVersionChecker.IsNewReleaseAvailable(recognizedVersion.major, recognizedVersion.minor, recognizedVersion.patch);

        UpdateClickedCommand = ReactiveCommand.Create(OnUpdateButtonClicked);
        EveSquadronPlayers = new DataGridCollectionView(_eveSquadronPlayerInformation);
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

    //This whole method (+Event) is a workaround to provide a point where the IDs are loaded and we can work with the data from there. Each ID causes enumerations over the whole collection and UI refresh - try to get rid of that in the (close) future!
    private void OnParsedNewID(object? sender, (int? CorporationID, int? AllianceID) newIDs)
    {
        if (newIDs.CorporationID is not null)
        {
            if (!_idCountDictionary.TryAdd(newIDs.CorporationID.Value, 1))
                _idCountDictionary[newIDs.CorporationID.Value] += 1;
                
            var corpMembers = _eveSquadronPlayerInformation.Where(x => x.Corporation?.ID == newIDs.CorporationID).ToList();
            foreach (var member in corpMembers)
                member.CorporationPasteCount = corpMembers.Count;
        }
        if (newIDs.AllianceID is not null)
        {
            if (!_idCountDictionary.TryAdd(newIDs.AllianceID.Value, 1))
                _idCountDictionary[newIDs.AllianceID.Value] += 1;
                
            var allianceMembers = _eveSquadronPlayerInformation.Where(x => x.Alliance?.ID == newIDs.AllianceID).ToList();
            foreach (var member in allianceMembers)
                member.AlliancePasteCount = allianceMembers.Count;
        }

        Dispatcher.UIThread.InvokeAsync(() => EveSquadronPlayers.Refresh());
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

    public DataGridCollectionView EveSquadronPlayers { get; }

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
            
            _eveSquadronPlayerInformation.Clear();
            _idCountDictionary.Clear();

            var clipboardSplit = Regex.Split(clipboardContent, "\r?\n")//This replaces Environment.NewLine and splits the newlines  because, for some reason, Windows used \n on a users system
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct()
                .ToList();

            var nameCharacterLimit = AppConstants.EveAPILimits.PostUniverseIDsNameCharacterLimit;
            if (clipboardSplit.Any(x => x.Length > nameCharacterLimit))
            {
                _logger.LogDebug($"TryParseClipboardContentForEve: Could not load player information. These arent proper eve names, the limit on characters in a name is {nameCharacterLimit}");
                return;
            }
            
            await foreach (var eveSquadronPlayerInformation in _playerInformationDataAggregator.GetAggregatedItemsFor(clipboardSplit))
            {
                _eveSquadronPlayerInformation.Add(eveSquadronPlayerInformation);
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
    
    #region zKillboard Navigation

    public void OpenZKillboardLinkFor(EveSquadronPlayerInformation target, string clickedColumn)
    {
        var targetUrl = clickedColumn switch
        {
            MainDataGridHeaderNames.CharacterName => $"{AppConstants.ZKillboardUrls.Character}/{target.Character.ID}",
            MainDataGridHeaderNames.Corporation when target.Corporation is not null => $"{AppConstants.ZKillboardUrls.Corporation}/{target.Corporation.ID}",
            MainDataGridHeaderNames.Alliance when target.Alliance is not null  =>  $"{AppConstants.ZKillboardUrls.Alliance}/{target.Alliance.ID}",
            _ => string.Empty
        };
        if (string.IsNullOrWhiteSpace(targetUrl)) return;
        
        Process.Start(new ProcessStartInfo(targetUrl) { UseShellExecute = true });
    }

    #endregion
}