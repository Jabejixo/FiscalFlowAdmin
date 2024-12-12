using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FiscalFlowAdmin.Converter;

public class NullableBoolToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b;
        return false; // Можно настроить для отображения `null` как `false` или оставить `null`
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b;
        return null;
    }
}