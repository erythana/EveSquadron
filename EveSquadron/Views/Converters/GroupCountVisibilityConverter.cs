using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace EveSquadron.Views.Converters;

public class GroupCountVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is >= 2; // only show when there are at least two people of a group

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}