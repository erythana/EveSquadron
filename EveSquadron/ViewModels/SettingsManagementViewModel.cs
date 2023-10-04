using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models;
using EveSquadron.Models.Enums;
using EveSquadron.Models.Options;
using EveSquadron.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveUI;
using EveSquadron.Extensions;

namespace EveSquadron.ViewModels;

public class SettingsManagementViewModel : ViewModelBase, ISettingsManagementViewModel
{
    #region member fields

    private readonly IApplicationSettingDataRepository _applicationSettingDataRepository;
    private readonly ILogger<ISettingsManagementViewModel> _logger;
    private int _clipboardPolling;
    private bool _autoExport;
    private bool _showPortrait;
    private bool _alwaysOnTop;
    private Color _hoverColor;
    private string _autoExportFile;
    private GridRowSizeEnum _gridRowSize;
    private ThemeVariant _theme;
    
    private Dictionary<string, string> _settingsToSave;
    private bool _whitelistActive;

    #endregion

    #region constructor

    public SettingsManagementViewModel(IServiceProvider serviceProvider, IApplicationSettingDataRepository applicationSettingDataRepository, ILogger<ISettingsManagementViewModel> logger)
    {
        _applicationSettingDataRepository = applicationSettingDataRepository;
        _logger = logger;
        _settingsToSave = new Dictionary<string, string>();

        SaveApplicationSettingsCommand = ReactiveCommand.CreateFromTask(OnSaveApplicationSettingsCommand);
        OpenAutoExportFolderPickerCommand = ReactiveCommand.CreateFromTask(OnOpenAutoExportFolderPickerCommand);
        MinimumClipboardPolling = AppConstants.MinimalClipboardPollingMs;
        MaximumClipboardPolling = AppConstants.MaximalClipboardPollingMs;
        AvailableGridRowSizes = Enum.GetValues<GridRowSizeEnum>();
        AvailableThemes = new List<ThemeVariant>
        {
            ThemeVariant.Default,
            ThemeVariant.Light,
            ThemeVariant.Dark
        };
        var colors = GetAllAvailableColors();
        AvailableHoverColors = colors;
        
        LoadAndSetSavedSettingsFromOptions(serviceProvider);
    }

    private async Task OnOpenAutoExportFolderPickerCommand()
    {
        try
        {
            if (Application.Current!.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime applicationLifetime ||
                applicationLifetime.Windows.FirstOrDefault(x => x.DataContext == this) is not { } window)
                return;
                
            var file = await GetCsvTargetFileFromSaveFilePickerAsync(window);

            var filePath = file?.Path.AbsolutePath;
            if (string.IsNullOrWhiteSpace(filePath))
                return;
            
            AutoExportFile = CreateFile(filePath);;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Could not open folder picker dialog!");
            throw;
        }
    }

    private static string CreateFile(string file)
    {
        var fileInfo = new FileInfo(file);
        if (!fileInfo.Exists && fileInfo.Directory!.HasWriteAccess(true))
            fileInfo.Create();

        return fileInfo.FullName;
    }

    private async Task OnSaveApplicationSettingsCommand()
    {
        try
        {
            var saveableSettings = GetListOfWriteableSettings();
            await _applicationSettingDataRepository.SaveApplicationSettings(saveableSettings);
            _settingsToSave.Clear();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Could not save application settings!");
            throw;
        }
    }
    
    #endregion

    #region properties

    public IReactiveCommand SaveApplicationSettingsCommand { get; }
    public IReactiveCommand OpenAutoExportFolderPickerCommand { get; }
    public int MinimumClipboardPolling { get; }
    public int MaximumClipboardPolling { get; }

    public int ClipboardPolling {
        get => _clipboardPolling;
        set
        {
            SetProperty(ref _clipboardPolling, value);
            AddToSaveableDictionary(EveSquadronOptions.Section, nameof(ClipboardPolling), value.ToString());
        }
    }

    public bool AutoExport {
        get => _autoExport;
        set
        {
            SetProperty(ref _autoExport, value);
            AddToSaveableDictionary(EveSquadronOptions.Section, nameof(AutoExport), value.ToString());
        }
    }

    public bool WhitelistActive {
        get => _whitelistActive;
        set
        {
            SetProperty(ref _whitelistActive, value);
            AddToSaveableDictionary(StatusOptions.Section, nameof(WhitelistActive), value.ToString());
        }
    }

    public bool ShowPortrait {
        get => _showPortrait;
        set
        {
            SetProperty(ref _showPortrait, value);
            AddToSaveableDictionary(EveSquadronOptions.Section, nameof(ShowPortrait), value.ToString());
        }
    }

    public bool AlwaysOnTop {
        get => _alwaysOnTop;
        set
        {
            SetProperty(ref _alwaysOnTop, value);
            AddToSaveableDictionary(StatusOptions.Section, nameof(AlwaysOnTop), value.ToString());
        }
    }

    public Color HoverColor {
        get => _hoverColor;
        set
        {
            SetProperty(ref _hoverColor, value);
            AddToSaveableDictionary(EveSquadronOptions.Section, nameof(HoverColor), value.ToString());
        }
    }

    public string AutoExportFile {
        get => _autoExportFile;
        set
        {
            SetProperty(ref _autoExportFile, value);
            AddToSaveableDictionary(EveSquadronOptions.Section, nameof(AutoExportFile), value);
        }
    }

    public GridRowSizeEnum GridRowSize {
        get => _gridRowSize;
        set
        {
            SetProperty(ref _gridRowSize, value);
            AddToSaveableDictionary(EveSquadronOptions.Section, nameof(GridRowSize), value.ToString());
        }
    }

    public ThemeVariant Theme {
        get => _theme;
        set
        {
            SetProperty(ref _theme, value);
            AddToSaveableDictionary(EveSquadronOptions.Section, nameof(Theme), value.ToString());
        }
    }

    public IEnumerable<ThemeVariant> AvailableThemes { get; set; }
    public IEnumerable<Color> AvailableHoverColors { get; set; }
    public IEnumerable<GridRowSizeEnum> AvailableGridRowSizes { get; set; }

    #endregion

    #region helper methods
    
    private static IEnumerable<Color> GetAllAvailableColors()
    {
        var properties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);
        var colors = properties.Where(x => x.PropertyType == typeof(Color)).Select(x => (Color)x.GetValue(null)!);
        return colors;
    }
    
    private void LoadAndSetSavedSettingsFromOptions(IServiceProvider serviceProvider)
    {
        var eveSquadronOptions = ResolveOptionsFromType<EveSquadronOptions>();
        var statusOptions = ResolveOptionsFromType<StatusOptions>();
        
        ClipboardPolling = int.TryParse(eveSquadronOptions.Value.ClipboardPollingMilliseconds, out var polling) ? polling : AppConstants.DefaultClipboardPollingMs;
        HoverColor = SettingConversionHelper.StringToColorConverter(eveSquadronOptions.Value.HoverColor);
        Theme = SettingConversionHelper.StringToThemeConverter(eveSquadronOptions.Value.Theme);
        AutoExportFile = eveSquadronOptions.Value.AutoExportFile;
        AutoExport = bool.TryParse(eveSquadronOptions.Value.AutoExport, out var autoExport) && autoExport;
        ShowPortrait = bool.TryParse(eveSquadronOptions.Value.ShowPortrait, out var showPortrait) && showPortrait;
        AlwaysOnTop = bool.TryParse(statusOptions.Value.AlwaysOnTop, out var alwaysOnTop) && alwaysOnTop;
        WhitelistActive = bool.TryParse(statusOptions.Value.WhitelistActive, out var whitelistActive) && whitelistActive;

        GridRowSize = Enum.TryParse(eveSquadronOptions.Value.GridRowSize, out GridRowSizeEnum rowSize)
            ? rowSize
            : AppConstants.DefaultGridRowSize;
        
        IOptions<T> ResolveOptionsFromType<T>() where T : class => (IOptions<T>)serviceProvider.GetService(typeof(IOptions<T>))!;
    }

    private void AddToSaveableDictionary(string sectionTarget, string? name, string? value)
    {
        if (name is null || value is null)
            return;
        
        if (!_settingsToSave.TryAdd($"{sectionTarget}:{name}", value))
            _settingsToSave[$"{sectionTarget}:{name}"] = value;
    }
    
    private IEnumerable<ConfigurationValue> GetListOfWriteableSettings() => 
        _settingsToSave.Select(x => new ConfigurationValue(){Name = x.Key, Value = x.Value});

    private async static Task<IStorageFile?> GetCsvTargetFileFromSaveFilePickerAsync(TopLevel topLevel) => await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        { 
            Title = "Save into CSV File",
            DefaultExtension = "*.csv",
            SuggestedFileName = "EveSquadron-Export.csv",
            FileTypeChoices = new List<FilePickerFileType>{ new("CSV")
            {
                Patterns = new[]{"*.csv"},
                MimeTypes = new[]{"text/*"},
            }},
            ShowOverwritePrompt = true
        });
    
    #endregion
}