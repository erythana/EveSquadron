using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;
using EveSquadron.Models;
using EveSquadron.Models.Enums;
using EveSquadron.Models.Interfaces;
using ReactiveUI;

namespace EveSquadron.ViewModels.Interfaces;

public interface IWhitelistManagementViewModel : INotifyPropertyChanged
{
    public ReactiveCommand<Unit, Unit> SaveWhitelistCommand { get; }
    
    public ReactiveCommand<Unit, Unit> AddSingleItemWhitelistCommand { get; }
    
    public ReactiveCommand<IList, Unit> DeleteSelectedWhitelistCommand { get; }
    
    public ReactiveCommand<Unit, Task> ImportClipboardWhitelistCommand { get; }
    
    public IEnumerable<EntityTypeEnum> AvailableEntityTypes { get; }
    
    public ObservableCollection<IWhitelistEntry> CurrentWhitelistEntries { get; }
    
    public bool IsWindowVisible { get; set; }
}