using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using static VehicleTrakker.DataDefinitions.Reminder;

namespace VehicleTrakker.Converters
{
    public class ReminderStateToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = (ReminderState)value;
            switch(state)
            {
                case ReminderState.Idle:
                    return "/Images/Reminders/ok.png";
                case ReminderState.Confirmed:
                    return "/Images/Reminders/warning.png";
                case ReminderState.Expired:
                    return "/Images/Reminders/fatal.png";
                default:
                    throw new ArgumentException("Invalid enem type");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ReminderStateToImageTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = (ReminderState)value;
            switch (state)
            {
                case ReminderState.Idle:
                    return "Not expired";
                case ReminderState.Confirmed:
                    return "Expired but confirmed";
                case ReminderState.Expired:
                    return "Expired";
                default:
                    throw new ArgumentException("Invalid enem type");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
