using IDUNv2.DataAccess;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace IDUNv2.Converters
{
    class StateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((LoadingState)value == LoadingState.Idle)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        // Not used
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return LoadingState.Finished;
        }
    }
}
