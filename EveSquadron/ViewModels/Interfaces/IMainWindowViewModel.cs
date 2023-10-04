using System.IO;
using Avalonia.Media;
using Avalonia.Styling;
using EveSquadron.Models.Enums;
using EveSquadron.Models.EveSquadron;

namespace EveSquadron.ViewModels.Interfaces;

public interface IMainWindowViewModel
{
    public IStatusBarViewModel StatusBarViewModel { get; }
    
    public IWhitelistManagementViewModel WhitelistManagementViewModel { get; }
    
    public ISettingsManagementViewModel SettingsManagementViewModel { get; }
    
    public ThemeVariant ThemeVariant { get; set; }
    
    public Color HoverColor { get; }
    
    public bool ShowPortrait { get; }

    public GridRowSizeEnum GridRowHeight { get; }
    
    public bool AutoExport { get; }
    public string AutoExportFile { get; }
    
    void OpenZKillboardLinkFor(EveSquadronPlayerInformation playerInformation, EntityTypeEnum clickedColumn);
}