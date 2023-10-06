using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Media;
using Avalonia.Styling;
using EveSquadron.Models.Enums;
using ReactiveUI;

namespace EveSquadron.ViewModels.Interfaces;

public interface ISettingsManagementViewModel : INotifyPropertyChanged
{
    public IReactiveCommand SaveApplicationSettingsCommand { get; }
    public IReactiveCommand OpenExportFilePickerCommand { get; }
    public IReactiveCommand ClearExportFileCommand { get; }
    public int MinimumClipboardPolling { get; }
    public int MaximumClipboardPolling { get; }
    public int ClipboardPolling { get; }
    public IEnumerable<ThemeVariant> AvailableThemes { get; }
    public ThemeVariant Theme { get; }
    public IEnumerable<Color> AvailableHoverColors { get; }
    public Color HoverColor { get; }
    public bool AutoExport { get; }
    public string ExportFile { get; }
    public bool WhitelistActive { get; }
    public bool ShowPortrait { get; }
    public bool AlwaysOnTop { get; }
    public IEnumerable<GridRowSizeEnum> AvailableGridRowSizes { get; }
    public GridRowSizeEnum GridRowSize { get; }
}