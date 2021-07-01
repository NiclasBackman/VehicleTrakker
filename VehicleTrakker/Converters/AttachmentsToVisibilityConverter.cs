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
    public class AttachmentsToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var attachments = value as List<Attachment>;
            if(attachments == null)
            {
                return Visibility.Collapsed;
            }

            return attachments.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
