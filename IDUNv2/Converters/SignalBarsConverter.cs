using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{
    class SignalBarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((byte)value == 1)
                return "\xE905";
            else if ((byte)value == 2)
                return "\xE906";
            else if ((byte)value == 3)
                return "\xE907";
            else if ((byte)value == 4)
                return "\xE908";
            else
                return "\xE904";
        }

        // Not Used
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return "\xE904";
        }
    }
}
