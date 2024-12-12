using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace FiscalFlowAdmin.Converter;

public class CollectionToStringConverter : IValueConverter
{
    public string DisplayMember { get; set; } = "Id";

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable enumerable)
        {
            var items = enumerable.Cast<object>()
                .Select(item => item.GetType().GetProperty(DisplayMember)?.GetValue(item)?.ToString())
                .Where(str => !string.IsNullOrEmpty(str));
            return string.Join(", ", items);
        }
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}