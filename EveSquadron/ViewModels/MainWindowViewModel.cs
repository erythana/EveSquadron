using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using EveSquadron.Models;
using EveSquadron.Models.Enums;
using EveSquadron.Models.EveSquadron;
using EveSquadron.Models.Interfaces;
using EveSquadron.Models.Options;
using EveSquadron.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EveSquadron.ViewModels;

public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    #region member fields

    private readonly ObservableCollection<EveSquadronPlayerInformation> _eveSquadronPlayerInformation;
    private readonly IPlayerInformationDataAggregator _playerInformationDataAggregator;
    private readonly Dictionary<int, int> _idCountDictionary;
    private readonly ILogger<MainWindowViewModel> _logger;
    private string? _previousClipboardContent = string.Empty;
    private DispatcherTimer _dispatcherTimer;
    private ThemeVariant _themeVariant;
    private Color _hoverColor;
    private bool _showPortrait;
    private int _gridRowHeight;

    #endregion

    #region constructor

    public MainWindowViewModel(IStatusBarViewModel statusBarViewModel, IWhitelistManagementViewModel whitelistManagementViewModel, ISettingsManagementViewModel settingsManagementViewModel, IPlayerInformationDataAggregator playerInformationDataAggregator,
        IOptions<EveSquadronOptions> eveSquadronOptions, ILogger<MainWindowViewModel> logger)
    {
        _idCountDictionary = new Dictionary<int, int>();
        _eveSquadronPlayerInformation = new ObservableCollection<EveSquadronPlayerInformation>();
        _playerInformationDataAggregator = playerInformationDataAggregator;
        _playerInformationDataAggregator.ParsedNewID += OnParsedNewID;
        _playerInformationDataAggregator.OnValidPaste += (_, _) =>
        {
            _eveSquadronPlayerInformation.Clear();
            _idCountDictionary.Clear();
        };
        _logger = logger;

        StatusBarViewModel = statusBarViewModel;
        StatusBarViewModel.PropertyChanged += OnStatusBarViewModelOnPropertyChanged;

        WhitelistManagementViewModel = whitelistManagementViewModel;
        WhitelistManagementViewModel.PropertyChanged += OnWhitelistManagementViewModelOnPropertyChanged;
        
        SettingsManagementViewModel = settingsManagementViewModel;
        SettingsManagementViewModel.PropertyChanged += OnSettingsManagementViewModelOnPropertyChanged;

        EveSquadronPlayers = new DataGridCollectionView(_eveSquadronPlayerInformation);

        EveSquadronPlayers.Filter = WhitelistFilter;

        InitializeFrom(eveSquadronOptions);
    }

    private void InitializeFrom(IOptions<EveSquadronOptions> options)
    {
        HoverColor = SettingConversionHelper.StringToColorConverter(options.Value.HoverColor);
        ShowPortrait = !bool.TryParse(options.Value.ShowPortrait, out var showPortrait) || showPortrait;
        GridRowHeight = GetGridSize(options.Value.GridRowSize);
        var timerTickRate = TryParseClipboardPollingRate(options.Value.ClipboardPollingMilliseconds);
        _dispatcherTimer = new DispatcherTimer(timerTickRate, DispatcherPriority.Background, ClipboardPollingTimerCallback)
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

        if (WhitelistManagementViewModel.IsWindowVisible) // TODO: More generic solution for multiple windows -- prevent any further updates when the window is active - if it has been active we still have set the previous content so we don't update
            return;

        await TryParseClipboardContentForEve(clipboardContent);
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

    #region properties

    public Color HoverColor {
        get => _hoverColor;
        private set => SetProperty(ref _hoverColor, value);
    }

    public bool ShowPortrait {
        get => _showPortrait;
        private set => SetProperty(ref _showPortrait, value);
    }

    public int GridRowHeight {
        get => _gridRowHeight;
        private set => SetProperty(ref _gridRowHeight, value);
    }
    
    public ThemeVariant ThemeVariant {
        get => _themeVariant;
        set => SetProperty(ref _themeVariant, value);
    }

    // ReSharper disable once InconsistentNaming

    public DataGridCollectionView EveSquadronPlayers { get; }

    public IStatusBarViewModel StatusBarViewModel { get; set; }

    public IWhitelistManagementViewModel WhitelistManagementViewModel { get; }
    
    public ISettingsManagementViewModel SettingsManagementViewModel { get; }

    #endregion

    #region helper methods

    private bool WhitelistFilter(object player)
    {
        if (player is not EveSquadronPlayerInformation playerInformation)
            return false;

        var excludedPlayers = WhitelistManagementViewModel.CurrentWhitelistEntries.Where(x => x.Type == EntityTypeEnum.Character);
        var excludedCorporations = WhitelistManagementViewModel.CurrentWhitelistEntries.Where(x => x.Type == EntityTypeEnum.Corporation);
        var excludedAlliances = WhitelistManagementViewModel.CurrentWhitelistEntries.Where(x => x.Type == EntityTypeEnum.Alliance);

        var isExcludedPlayer = excludedPlayers.Any(x => x.Name == playerInformation.Character.Name);
        var isExcludedCorp = excludedCorporations.Any(x => x.Name == playerInformation.Corporation?.Name);
        var isExcludedAlliance = excludedAlliances.Any(x => x.Name == playerInformation.Alliance?.Name);

        return !StatusBarViewModel.WhitelistActive || !(isExcludedPlayer || isExcludedCorp || isExcludedAlliance);
    }

    private static TimeSpan TryParseClipboardPollingRate(string? configuredPollingRate)
    {
        if (double.TryParse(configuredPollingRate, out var pollingRate))
            pollingRate = Math.Clamp(pollingRate, AppConstants.MinimalClipboardPollingMs, AppConstants.MaximalClipboardPollingMs);
        else
            pollingRate = AppConstants.DefaultClipboardPollingMs;

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

            var clipboardSplit = Regex.Split(clipboardContent, "\r?\n") //This replaces Environment.NewLine and splits the newlines  because, for some reason, Windows used \n on a users system
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

            await foreach (var eveSquadronPlayerInformation in _playerInformationDataAggregator.GetAggregatedItemsFor(clipboardSplit, ShowPortrait))
            {
                _eveSquadronPlayerInformation.Add(eveSquadronPlayerInformation);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not load player information. Check input from clipboard!.");
        }
    }



    private int GetGridSize(string? sizeMode = "big") => sizeMode?.ToLower() switch
    {
        "small" => 16,
        "medium" => 24,
        _ => 32,
    };

    #endregion

    #region zKillboard Navigation

    public void OpenZKillboardLinkFor(EveSquadronPlayerInformation target, EntityTypeEnum clickedColumn)
    {
        var targetUrl = clickedColumn switch
        {
            EntityTypeEnum.Character => $"{AppConstants.ZKillboardUrls.Character}/{target.Character.ID}",
            EntityTypeEnum.Corporation when target.Corporation is not null => $"{AppConstants.ZKillboardUrls.Corporation}/{target.Corporation.ID}",
            EntityTypeEnum.Alliance when target.Alliance is not null => $"{AppConstants.ZKillboardUrls.Alliance}/{target.Alliance.ID}",
            _ => string.Empty
        };
        if (string.IsNullOrWhiteSpace(targetUrl)) return;

        Process.Start(new ProcessStartInfo(targetUrl)
        {
            UseShellExecute = true
        });
    }

    #endregion

    #region ViewModel INPC Handler

    private void OnSettingsManagementViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not ISettingsManagementViewModel settingsManagementViewModel)
            return;
        
        switch (e.PropertyName)
        {
            case nameof(SettingsManagementViewModel.Theme):
                ThemeVariant = settingsManagementViewModel.Theme;
                break;
            case nameof(SettingsManagementViewModel.HoverColor):
                HoverColor = settingsManagementViewModel.HoverColor;
                break;
            case nameof(SettingsManagementViewModel.AlwaysOnTop):
                StatusBarViewModel.AlwaysOnTop = settingsManagementViewModel.AlwaysOnTop;
                break;
            case nameof(settingsManagementViewModel.WhitelistActive):
                StatusBarViewModel.WhitelistActive = settingsManagementViewModel.WhitelistActive;
                break;
            case nameof(settingsManagementViewModel.ShowPortrait):
                ShowPortrait = settingsManagementViewModel.ShowPortrait;
                break;
        }
    }

    private void OnWhitelistManagementViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WhitelistManagementViewModel.IsWindowVisible))
        {
            _dispatcherTimer.IsEnabled = !WhitelistManagementViewModel.IsWindowVisible;
            EveSquadronPlayers?.Refresh();
        }
    }

    private void OnStatusBarViewModelOnPropertyChanged(object? _, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(StatusBarViewModel.WhitelistActive))
            EveSquadronPlayers?.Refresh();
    }

    #endregion
}