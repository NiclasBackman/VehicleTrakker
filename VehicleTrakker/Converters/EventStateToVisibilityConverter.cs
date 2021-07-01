using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NavigationTest
{
    class EventStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var enumValue = (VehiclePersistenceState)value;
            if (enumValue == VehiclePersistenceState.Saved)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
