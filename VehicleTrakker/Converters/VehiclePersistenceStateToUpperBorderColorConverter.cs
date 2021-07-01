using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace NavigationTest
{
    class VehiclePersistenceStateToUpperBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var e = (VehiclePersistenceState)value;
            switch(e)
            {
                case VehiclePersistenceState.Prepared:
                    return /*new SolidColorBrush*/(Colors.LightBlue);
                case VehiclePersistenceState.Saved:
                    return /*new SolidColorBrush*/(Colors.LightGreen);
                case VehiclePersistenceState.Edited:
                    return /*new SolidColorBrush*/(Colors.Orange);
                default:
                    throw new ArgumentException("Invalid enum type");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class VehiclePersistenceStateToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var e = (VehiclePersistenceState)value;
            switch (e)
            {
                case VehiclePersistenceState.Prepared:
                    return new SolidColorBrush(Colors.LightBlue);
                case VehiclePersistenceState.Saved:
                    return new SolidColorBrush(Colors.LightGreen);
                case VehiclePersistenceState.Edited:
                    return new SolidColorBrush(Colors.Orange);
                default:
                    throw new ArgumentException("Invalid enum type");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
