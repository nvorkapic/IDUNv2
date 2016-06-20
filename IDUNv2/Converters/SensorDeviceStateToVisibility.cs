using IDUNv2.SensorLib;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{
    public class SensorDeviceStateToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Collapsed;

            var state = (SensorDeviceState)value;
            if (state == SensorDeviceState.Offline)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Visibility.Collapsed;
        }
    }
}
