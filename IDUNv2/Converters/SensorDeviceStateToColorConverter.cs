using IDUNv2.SensorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{
    public class SensorDeviceStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = (SensorDeviceState)value;
            switch (state)
            {
                case SensorDeviceState.Offline:
                    return "DarkGray";
                case SensorDeviceState.Online:
                    return "#FF00CC00";
                case SensorDeviceState.Simulated:
                    return "Gold";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return SensorDeviceState.Offline;
        }
    }
}
