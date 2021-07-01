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
    public class ActionEventToLocationVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                //throw new InvalidOperationException("null in value converter");
                return Visibility.Visible;
            }
            var evt = value as ActionEvent;
            if (evt == null)
            {
                return Visibility.Visible;
            }
            else
            {
                if (evt.ActionType == ViewModels.ActionType.Acquired)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
