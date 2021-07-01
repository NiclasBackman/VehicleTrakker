using System;
using System.Linq;
using VehicleTrakker.DataDefinitions;
using Windows.UI.Xaml.Data;

namespace NavigationTest
{
    public class EventTypeToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var e = (EventType)value;

            switch(e)
            {
                case EventType.Fuel:
                case EventType.Charging:
                case EventType.Insurance:
                case EventType.TollFee:
                case EventType.Inspection:
                case EventType.Service:
                case EventType.Repair:
                case EventType.Expense:
                case EventType.Tax:
                case EventType.Wash:
                case EventType.Action:
                    return EventImageMapping.AllEvents.Where(x => x.Type == e).FirstOrDefault().ImageSource;
                default:
                    throw new ArgumentException("Unhandled event type: ");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
