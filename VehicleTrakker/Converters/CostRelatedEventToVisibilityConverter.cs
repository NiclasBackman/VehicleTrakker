using System;
using VehicleTrakker.DataDefinitions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.Converters
{
    public class CostRelatedEventToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var type = (EventType)value;
            return type == EventType.Action ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
