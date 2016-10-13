using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{
    class InternetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "\xE904";
            if ((bool)value)
                return "\xE909";
            else
                return "\xE8CD";
        }

        // Not Used
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }
}
