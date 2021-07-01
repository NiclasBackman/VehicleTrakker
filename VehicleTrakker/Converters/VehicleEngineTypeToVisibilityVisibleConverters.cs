using NavigationTest;
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
    public class VehicleEngineTypeIceToVisibilityVisibleConverter : IValueConverter
    {
        private VehicleService vehicleService;

        public VehicleEngineTypeIceToVisibilityVisibleConverter()
        {
            vehicleService = VehicleService.Instance;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var OrganizedEventPanelIsVisible = (bool)value;
            var vehicle = vehicleService.QueryVehicleById(vehicleService.SelectedVehicleId);
            if (vehicle == null)
            {
                return Visibility.Collapsed;
            }
            return vehicle.EngineType == EngineType.ICE && OrganizedEventPanelIsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class VehicleEngineTypeHybridToVisibilityVisibleConverter : IValueConverter
    {
        private VehicleService vehicleService;

        public VehicleEngineTypeHybridToVisibilityVisibleConverter()
        {
            vehicleService = VehicleService.Instance;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var OrganizedEventPanelIsVisible = (bool)value;
            var vehicle = vehicleService.QueryVehicleById(vehicleService.SelectedVehicleId);
            if (vehicle == null)
            {
                return Visibility.Collapsed;
            }
            return vehicle.EngineType == EngineType.Hybrid && OrganizedEventPanelIsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class VehicleEngineTypePureEvToVisibilityVisibleConverter : IValueConverter
    {
        private VehicleService vehicleService;

        public VehicleEngineTypePureEvToVisibilityVisibleConverter()
        {
            vehicleService = VehicleService.Instance;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var OrganizedEventPanelIsVisible = (bool)value;
            var vehicle = vehicleService.QueryVehicleById(vehicleService.SelectedVehicleId);
            if (vehicle == null)
            {
                return Visibility.Collapsed;
            }
            return vehicle.EngineType == EngineType.PureEv && OrganizedEventPanelIsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
