using Avalonia.Media;
using Avalonia.Styling;
using EveSquadron.Models;
using EveSquadron.Models.EveSquadron;

namespace EveSquadron.ViewModels.Interfaces;

public interface IMainWindowViewModel
{
    public IStatusBarViewModel StatusBarViewModel { get; }
    
    public IWhitelistManagementViewModel WhitelistManagementViewModel { get; }
    
    public Color HoverColor { get; }
    
    public bool ShowPortrait { get; }

    public int GridRowHeight { get; }
    
    void OpenZKillboardLinkFor(EveSquadronPlayerInformation playerInformation, EntityTypeEnum clickedColumn);
}