using IDUNv2.SensorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{
    public class SensorStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = (SensorState)value;
            switch (state)
            {
                case SensorState.Online:
                    return "Green";
                case SensorState.Offline:
                    return "DarkGray";
                case SensorState.Faulted:
                    return "Red";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
