using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using EveSquadron.Models;
using EveSquadron.Models.Interfaces;
using ReactiveUI;

namespace EveSquadron.ViewModels.Interfaces;

public interface IWhitelistManagementViewModel : INotifyPropertyChanged
{
    public IReactiveCommand SaveWhitelistCommand { get; }
    
    public IReactiveCommand AddSingleItemWhitelistCommand { get; }
    
    public IReactiveCommand DeleteSelectedWhitelistCommand { get; }
    
    public IReactiveCommand ImportClipboardWhitelistCommand { get; }
    
    public IEnumerable<EntityTypeEnum> AvailableEntityTypes { get; }
    
    public ObservableCollection<IWhitelistEntry> CurrentWhitelistEntries { get; }
    
    public bool IsWindowVisible { get; set; }
}