using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Styling;
using EveSquadron.Models;
using EveSquadron.Models.Enums;
using ReactiveUI;

namespace EveSquadron.ViewModels.Interfaces;

public interface ISettingsManagementViewModel : INotifyPropertyChanged
{
    public ReactiveCommand<Unit, Unit> SaveApplicationSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenExportFilePickerCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearExportFileCommand { get; }
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
    public bool CompactUI { get; }
    public IEnumerable<GridFontSizeEnum> AvailableGridFontSizes { get; }
    public GridFontSizeEnum GridFontSize { get; }
    public IEnumerable<DataGridOrderMapping> ColumnOrder { get; }
    public Task SaveColumnOrder(IEnumerable<DataGridOrderMapping> columnOrder);
    public WindowDimension? WindowDimension { get; }
    Task SaveWindowDimension(WindowDimension windowDimension);
}