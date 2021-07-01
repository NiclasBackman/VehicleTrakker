using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.Converters
{
    public class InspectionResultToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var e = (InspectionResultType)value;
            return e.ToDescriptionString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class InspectionResultToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var e = (InspectionResultType)value;
            switch (e)
            {
                case InspectionResultType.Passed:
                    return "/Images/Inspection/success.png";
                case InspectionResultType.PassedWithRemarks:
                    return "/Images/Inspection/warning.png";
                case InspectionResultType.Failed:
                    return "/Images/Inspection/fail.png";
                default:
                    throw new InvalidOperationException("Undefined enum type: " + e);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
