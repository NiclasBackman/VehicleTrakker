using System;
using VehicleTrakker.Services;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.Converters
{
    public class DistanceToLocalizedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var settings = SettingsService.Instance.QuerySettings();

            var val = int.Parse(value.ToString());
            var ret = val + " " + settings.DistanceUnit;
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
