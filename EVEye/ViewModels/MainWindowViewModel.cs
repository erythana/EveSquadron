using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData;
using EVEye.DataAccess;
using EVEye.DataAccess.Interfaces;
using EVEye.Models;
using EVEye.Models.EVE.Interfaces;
using EVEye.Models.ZKillboard.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVEye.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    #region member fields

    private readonly IPlayerInformationDataAggregator _playerInformationDataAggregator;
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly DispatcherTimer _clipboardPollingTimer;
    private string _previousClipboardContent = string.Empty;
    
    private bool _alwaysOnTop;

    #endregion

    #region constructor

    public MainWindowViewModel(IPlayerInformationDataAggregator playerInformationDataAggregator, IConfiguration configuration, ILogger<MainWindowViewModel> logger)
    {
        EVEyePlayerInformation = new ObservableCollection<EVEyePlayerInformation>();
        EVEyePlayerInformation.Add(new());
        
        _playerInformationDataAggregator = playerInformationDataAggregator;

        _logger = logger;

        var configuredPollingRate = configuration.GetValue<string>("ClipboardPollingMilliseconds");
        var timerTickRate = TryParseClipboardPollingRate(configuredPollingRate);
        
        _clipboardPollingTimer = new DispatcherTimer(timerTickRate, DispatcherPriority.Background, ClipboardPollingTimerCallback)
        {
            IsEnabled = true
        };
    }
    
    #endregion

    #region properties

    public bool AlwaysOnTop {
        get => _alwaysOnTop;
        set => SetProperty(ref _alwaysOnTop, value);
    }

    // ReSharper disable once InconsistentNaming
    public ObservableCollection<EVEyePlayerInformation> EVEyePlayerInformation { get; set; }

    #endregion

    #region event handler

    private async void ClipboardPollingTimerCallback(object? sender, EventArgs e)
    {
        var clipboardContent = await Avalonia.Application.Current!.Clipboard!.GetTextAsync();
        if(clipboardContent == _previousClipboardContent)
            return;
        _previousClipboardContent = clipboardContent;
        
        //TODO: MAYBE PAUSE TIMER WHILE DOING THE REST
        await TryParseClipboardContentForEve(clipboardContent);
    }
    
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
            
            var playerInformation = await _playerInformationDataAggregator.GetAggregatedItemsFor(clipboardContent.Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
            EVEyePlayerInformation.Clear();
            EVEyePlayerInformation.AddRange(playerInformation);
            RaisePropertyChanged(nameof(Models.EVEyePlayerInformation));
        }
        catch (Exception e)
        {
            _logger.LogInformation(e, "Could not load player information. Probably no eve-related data in clipboard.");
        }
    }

    #endregion
}