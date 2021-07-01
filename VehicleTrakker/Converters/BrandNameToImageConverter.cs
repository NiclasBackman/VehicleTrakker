using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;
using Windows.UI.Xaml.Data;

namespace NavigationTest
{
    class BrandNameToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Vehicle vehicle)
            {
                return VehicleBrand.AllVehicleBrands.FirstOrDefault(x => x.Name == vehicle.Brand)?.Image;
            }
            return "/Images/no_selection.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    class ExplicitBrandNameToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string brandName)
            {
                return VehicleBrand.AllVehicleBrands.FirstOrDefault(x => x.Name == brandName)?.Image;
            }
            return "/Images/no_selection.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
