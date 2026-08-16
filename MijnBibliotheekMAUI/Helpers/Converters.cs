using System.Globalization;

namespace MijnBibliotheekMAUI.Helpers;

// Converteert een bool naar het omgekeerde: True wordt False en andersom
// Gebruik: IsVisible="{Binding IsTeruggebracht, Converter={x:StaticResource InverseBoolConverter}}"
public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
}

// Converteert een string naar een bool: leeg = False, gevuld = True
// Gebruik: IsVisible="{Binding Error, Converter={x:StaticResource StringNotEmptyConverter}}"
public class StringNotEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string s && !string.IsNullOrWhiteSpace(s);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => null;
}
