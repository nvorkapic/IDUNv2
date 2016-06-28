using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{
    public class SensorTriggerIdToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? false : (int)value != 0;
        }

        // not used
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return 0;
        }
    }
}
