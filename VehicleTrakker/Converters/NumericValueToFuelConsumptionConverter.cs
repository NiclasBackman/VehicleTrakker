using System;
using VehicleTrakker.Services;
using Windows.UI.Xaml.Data;
using static VehicleTrakker.DataDefinitions.Settings;

namespace VehicleTrakker.Converters
{
    public class NumericValueToFuelConsumptionConverter : IValueConverter
    {
        private SettingsService settingsService;

        public NumericValueToFuelConsumptionConverter()
        {
            settingsService = SettingsService.Instance;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var settings = settingsService.QuerySettings();
            string val = String.Format("{0:0.00}", float.Parse(value.ToString()));
            return val + " " + settings.FuelConsumption.ToDescriptionString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class NumericValueToEnergyConsumptionConverter : IValueConverter
    {
        private SettingsService settingsService;

        public NumericValueToEnergyConsumptionConverter()
        {
            settingsService = SettingsService.Instance;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var settings = settingsService.QuerySettings();
            string val = String.Format("{0:0.00}", float.Parse(value.ToString()));
            if(settings.FuelConsumption == FuelConsumptionType.LiterPer10km)
            {
                return val + " " + settings.EnergyUnit + "/10" + settings.DistanceUnit;
            }
            else
            {
                return val + " " + settings.DistanceUnit + "/" + settings.EnergyUnit;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
