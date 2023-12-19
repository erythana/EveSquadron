using System;
using System.Collections.Generic;
using System.Text.Json;
using Avalonia.Media;
using Avalonia.Styling;

namespace EveSquadron.Models.Helper;

public static class SettingConversionHelper
{
    public static ThemeVariant StringToThemeConverter(string themeInput) => themeInput switch
    {
        { } theme when theme.Equals("Dark", StringComparison.InvariantCultureIgnoreCase) => ThemeVariant.Dark,
        { } theme when theme.Equals("Light", StringComparison.InvariantCultureIgnoreCase) => ThemeVariant.Light,
        _ => ThemeVariant.Default
    };
    
    public static Color StringToColorConverter(string colorInput) => Color.TryParse(colorInput, out var color)
        ? color
        : AppConstants.DefaultHoverColor;

    public static IEnumerable<DataGridOrderMapping> StringToColumnOrderConverter(string columnMapping)
    {
        if (string.IsNullOrWhiteSpace(columnMapping))
            return Array.Empty<DataGridOrderMapping>();
        return JsonSerializer.Deserialize<IEnumerable<DataGridOrderMapping>>(columnMapping) ?? Array.Empty<DataGridOrderMapping>();
    }

    public static WindowDimension? StringToWindowDimensionConverter(string windowDimension)
    {
        return string.IsNullOrWhiteSpace(windowDimension)
            ? null
            : JsonSerializer.Deserialize<WindowDimension>(windowDimension);
    }
}