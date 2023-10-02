using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Styling;
using ReactiveUI;

namespace EveSquadron.ViewModels.Interfaces;

public interface IStatusBarViewModel : INotifyPropertyChanged
{
    public ThemeVariant ThemeVariant { get; set; }
    
    public bool WhitelistActive { get; set; }
    
    public bool AlwaysOnTop { get; set; }

    public ReactiveCommand<Unit, Unit> OpenUpdateCommand { get; }
    
    public ReactiveCommand<Unit, Unit> OpenWhitelistCommand { get; }
    
    public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }

    public Task<bool?> UpdateAvailable { get; }

    public string LatestReleasePath { get; }
}