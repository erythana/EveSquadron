using System;
using EveSquadron.ViewModels.Interfaces;

namespace EveSquadron.ViewModels;

public sealed class DesignTimeMainWindowViewModel : MainWindowViewModel
{
    public DesignTimeMainWindowViewModel() : base(null, null, null, null, null)
    {
        if (!Avalonia.Controls.Design.IsDesignMode)
            throw new PlatformNotSupportedException("This ViewModel is only allowed during DesignTime");
    }
}