using IDUNv2.SensorLib;
using System;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{
    public class SensorToGaugeIndicatorColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = (SensorFaultState)value;
            if (state == SensorFaultState.Faulted)
                return "DarkRed";
            return "Transparent";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
