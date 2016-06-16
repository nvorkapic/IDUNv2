using System;
using System.Globalization;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{
    public sealed class NullToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        // Not used
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Visibility.Visible;
        }
    }
}
