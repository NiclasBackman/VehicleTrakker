using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.Converters
{
    public class ActionTypeToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var e = (ActionType)value;
            return e.ToDescriptionString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
