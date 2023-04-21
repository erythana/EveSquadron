using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.Models.EveSquadron;
using EveSquadron.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace EveSquadron.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    #region member fields

    private readonly IPlayerInformationDataAggregator _playerInformationDataAggregator;
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly DispatcherTimer _clipboardPollingTimer;
    private string? _previousClipboardContent = string.Empty;

    private bool _alwaysOnTop;
    private ThemeVariant _themeVariant;

    #endregion

    #region constructor

    public MainWindowViewModel(IPlayerInformationDataAggregator playerInformationDataAggregator, IAppSettingsLoader settings, ILogger<MainWindowViewModel> logger)
    {
        _themeVariant = ThemeVariant.Default;
        _playerInformationDataAggregator = playerInformationDataAggregator;
        _logger = logger;

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

            EveSquadronPlayerInformation.Clear();
            
            await foreach (var eveSquadronPlayerInformation in _playerInformationDataAggregator.GetAggregatedItemsFor(clipboardContent
                               .Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Distinct()))
            {
                EveSquadronPlayerInformation.Add(eveSquadronPlayerInformation);
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation(e, "Could not load player information. Probably no eve-related data in clipboard.");
        }
    }

    #endregion
}