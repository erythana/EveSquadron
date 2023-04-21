using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EveSquadron.Models;

public class ModelBase : INotifyPropertyChanged
{
    #region Events

    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion Events

    #region Methods

    protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected virtual void OnPropertiesChanged(params string[] properties)
    {
        foreach (var property in properties)
        {
            RaisePropertyChanged(property);
        }
    }

    protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;
        storage = value;
        RaisePropertyChanged(propertyName);
        return true;
    }

    #endregion
}