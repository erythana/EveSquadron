using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using EVEye.Models;
using EVEye.Models.EVE.Interfaces;
using EVEye.Models.ZKillboard.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVEye.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    #region member fields

    private readonly IZKillboardDataRepository _zKillboardDataRepository;
    private readonly IEveDataRepository _eveDataRepository;
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly DispatcherTimer _clipboardPollingTimer;
    private string _previousClipboardContent = string.Empty;
    
    private bool _alwaysOnTop;

    #endregion

    #region constructor

    public MainWindowViewModel(IZKillboardDataRepository zKillboardDataRepository, IEveDataRepository eveDataRepository, IConfiguration configuration, ILogger<MainWindowViewModel> logger)
    {
        EVEyePlayerInformation = new List<EVEyePlayerInformation>();
        EVEyePlayerInformation.AddRange(Enumerable.Range(0, 10).Select<int, EVEyePlayerInformation>(x => new EVEyePlayerInformation(){ID = x, CharacterName = "Test Char"}));
        
        _zKillboardDataRepository = zKillboardDataRepository;
        _eveDataRepository = eveDataRepository;
        _logger = logger;
        
        var configuredPollingRate = configuration["clipboardPollingMilliseconds"];
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
    public List<EVEyePlayerInformation> EVEyePlayerInformation { get; set; }

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
    
    private Task TryParseClipboardContentForEve(string clipboardContent)
    {
        _logger.LogDebug($"TICK TACK: {clipboardContent}");

        return Task.CompletedTask;
    }

    #endregion
}