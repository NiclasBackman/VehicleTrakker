using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.Services;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.Converters
{
    public class NumericValueToMoneyConverter : IValueConverter
    {
        private SettingsService settingsService;

        public NumericValueToMoneyConverter()
        {
            settingsService = SettingsService.Instance;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var settings = settingsService.QuerySettings();
            var val = float.Parse(value.ToString());
            var ret = val.ToString("C", System.Globalization.CultureInfo.GetCultureInfo(settings.CurrencyCultureCode));
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
