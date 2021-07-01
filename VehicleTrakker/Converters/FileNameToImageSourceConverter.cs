using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.Converters
{
    public class FileNameToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var fileName = value as string;
            if(fileName == null)
            {
                return string.Empty;
            }

            fileName = fileName.ToLower();
            if (fileName.EndsWith(".bmp"))
            {
                return "/Images/FileTypes/bmp-icon.png";
            }
            else if(fileName.EndsWith(".doc") || fileName.EndsWith(".docx"))
            {
                return "/Images/FileTypes/docx-win-icon.png";
            }
            else if (fileName.EndsWith(".gif"))
            {
                return "/Images/FileTypes/gif-icon.png";
            }
            else if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg"))
            {
                return "/Images/FileTypes/jpeg-icon.png";
            }
            else if (fileName.EndsWith(".txt"))
            {
                return "/Images/FileTypes/text-icon.png";
            }
            else if (fileName.EndsWith(".pdf"))
            {
                return "/Images/FileTypes/pdf-icon.png";
            }
            else if (fileName.EndsWith(".xls") || fileName.EndsWith(".xlsx"))
            {
                return "/Images/FileTypes/xlsx-win-icon.png";
            }
            else
            {
                throw new ArgumentException("Unhandled file type: " + fileName);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
