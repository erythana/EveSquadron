using System;
using System.Globalization;
using Avalonia.Data.Converters;
using EveSquadron.Models.Helper;

namespace EveSquadron.Views.Converters;

public class ExportFilePathToVisibleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var filePath = value as string;
        return !string.IsNullOrWhiteSpace(filePath) && ExportFileHelper.IsValidExportFile(filePath);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}