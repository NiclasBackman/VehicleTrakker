using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.Converters
{
    public class VehicleEngineTypeToFuelStatisticsVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var vehicle = value as Vehicle;
            if(vehicle == null)
            {
                return Visibility.Collapsed;
                //throw new InvalidOperationException("Vehicle is not set");
            }
            if(vehicle.EngineType == NavigationTest.EngineType.ICE || vehicle.EngineType == NavigationTest.EngineType.Hybrid)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class VehicleEngineTypeToChargingStatisticsVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var vehicle = value as Vehicle;
            if (vehicle == null)
            {
                return Visibility.Collapsed;
                //throw new InvalidOperationException("Vehicle is not set");
            }
            if (vehicle.EngineType == NavigationTest.EngineType.PureEv || vehicle.EngineType == NavigationTest.EngineType.Hybrid)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
