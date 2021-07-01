using System;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Sales_Dashboard
{
    public class CustomSeriesDescriptor : ChartSeriesDescriptor
    {
       public DataTemplate IntersectionPointTemplate { get; set; }
       public DataTemplate TrackInfoTemplate { get; set; }   

        protected override ChartSeries CreateInstanceCore(object context)
        {
            var data = context as Data;

            LineSeries lineSeries = new LineSeries();
            lineSeries.ValueBinding = new PropertyNameDataPointBinding("Amount");
            lineSeries.CategoryBinding = new PropertyNameDataPointBinding("TimeStamp");

            ChartTrackBallBehavior.SetIntersectionTemplate(lineSeries, IntersectionPointTemplate);

            ChartTrackBallBehavior.SetTrackInfoTemplate(lineSeries, TrackInfoTemplate);

            return lineSeries;
        }

        public override Type DefaultType
        {
            get { return typeof(LineSeries); }
        }
    }
    public class IntersectionPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var info = value as DataPointInfo;
            var palette = parameter as ChartPalette;

            var brush = palette.GetBrush(info.Series.ActualPaletteIndex, PaletteVisualPart.Fill);
         
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
