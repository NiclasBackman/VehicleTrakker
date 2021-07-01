using System;
using VehicleTrakker.DataDefinitions;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.Converters
{
    public class EventTypeToDescriptionTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var e = (EventType)value;
            return e.ToDescriptionString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
