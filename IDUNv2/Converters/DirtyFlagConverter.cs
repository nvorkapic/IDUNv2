using System;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{
    public class DirtyFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            return (bool)value ? "*" : "";
        }

        // Not used
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }
}
