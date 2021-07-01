using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.Converters
{
    public class AttachmentsToTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var attachments = value as List<Attachment>;
            if (attachments == null)
            {
                return string.Empty;
            }

            var res = attachments.Count + " attachment" + (attachments.Count > 0 ? "s" : string.Empty);
            //var res = attachments.Count > 0 ? $"{attachments.Count} attachment" +$"{ (attachments.Count > 0 ? "s" : string.Empty)}" : string.Empty;
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
