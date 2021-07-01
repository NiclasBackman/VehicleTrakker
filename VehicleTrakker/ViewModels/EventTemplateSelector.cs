using System;
using VehicleTrakker.DataDefinitions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NavigationTest
{
    public class EventTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommonTemplate { get; set; }

        public DataTemplate FuelTemplate { get; set; }

        public DataTemplate ChargingTemplate { get; set; }

        public DataTemplate InspectionTemplate { get; set; }

        public DataTemplate ActionTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if(item is FuelEvent)
            {
                return FuelTemplate;
            }
            else if(item is ChargingEvent)
            {
                return ChargingTemplate;
            }
            else if (item is InspectionEvent)
            {
                return InspectionTemplate;
            }
            else if (item is ActionEvent)
            {
                return ActionTemplate;
            }
            return CommonTemplate;
        }
    }
}
