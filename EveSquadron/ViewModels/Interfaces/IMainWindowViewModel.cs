using System.ComponentModel;
using System.Reactive;
using Avalonia.Media;
using Avalonia.Styling;
using EveSquadron.Models.Enums;
using EveSquadron.Models.EveSquadron;
using ReactiveUI;

namespace EveSquadron.ViewModels.Interfaces;

public interface IMainWindowViewModel : INotifyPropertyChanged
{
    public IStatusBarViewModel StatusBarViewModel { get; }
    
    public IWhitelistManagementViewModel WhitelistManagementViewModel { get; }
    
    public ISettingsManagementViewModel SettingsManagementViewModel { get; }
    
    public ReactiveCommand<EveSquadronPlayerInformation, Unit> ExportPlayerInformationCommand { get; }
    
    public ThemeVariant ThemeVariant { get; set; }
    
    public Color HoverColor { get; }
    
    public bool ShowPortrait { get; }

    public GridFontSizeEnum GridFontSize { get; }
    
    public double GridRowHeight { get; }
    
    public bool AutoExport { get; }
    
    public string ExportFilePath { get; }
    
    void OpenZKillboardLinkFor(EveSquadronPlayerInformation playerInformation, EntityTypeEnum clickedColumn);
}