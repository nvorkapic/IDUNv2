using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{

    public sealed class NotificationTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "";
            var type = (Notification)value;
            switch(type.Type)
                {
                case NotificationType.Error: return "\xE730";
                case NotificationType.Warning: return "\xE8C9";
                case NotificationType.Information: return "\xE8BD";
                case NotificationType.Tooltip: return "";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
