using NavigationTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.Converters
{
    public class EngineTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var e = (EngineType)value;
            switch(e)
            {
                case EngineType.ICE:
                    return "/Images/Vehicle/internal_combustion_engine.png";
                case EngineType.Hybrid:
                    return "/Images/Vehicle/plugin_hybrid.png";
                case EngineType.PureEv:
                    return "/Images/Vehicle/electric.png";
                default:
                    throw new ArgumentException("Unhandled EngineType: " + (int)value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
