using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Styling;

namespace EveSquadron.Views.Converters;

public class BooleanToDarkThemeVariantConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ThemeVariant themeVariant) return false;

        return themeVariant == ThemeVariant.Dark ? true : themeVariant == ThemeVariant.Light ? false : null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool isDark) return ThemeVariant.Default;

        return isDark
            ? ThemeVariant.Dark
            : ThemeVariant.Light;
    }
}