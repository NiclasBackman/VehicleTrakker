using System;
using System.Collections.ObjectModel;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.Converters
{
    public class AttachmentsCountToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var attachments = value as ObservableCollection<AttachmentViewModel>;
            if(attachments == null)
            {
                throw new InvalidOperationException("Unhandled object type, should be ObservableCollection<AttachmentViewModel>");
            }
            return attachments.Count > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
