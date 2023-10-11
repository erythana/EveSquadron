using System;
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

}