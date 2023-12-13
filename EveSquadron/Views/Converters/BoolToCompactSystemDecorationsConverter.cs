using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace EveSquadron.Views.Converters;

public class BoolToCompactSystemDecorationsConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isCompact = value is true;
        return isCompact
            ? SystemDecorations.BorderOnly
            : SystemDecorations.Full;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}