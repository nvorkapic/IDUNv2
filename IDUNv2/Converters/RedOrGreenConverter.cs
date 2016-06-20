using System;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{
    public class RedOrGreenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "Red";
            if ((bool)value)
                return "Red";
            else
                return "Green";
        }

        // Not Used
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }
}
