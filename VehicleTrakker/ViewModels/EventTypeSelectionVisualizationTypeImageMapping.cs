using System.Collections.ObjectModel;
using static VehicleTrakker.DataDefinitions.Settings;

namespace VehicleTrakker.ViewModels
{
    public class EventTypeSelectionVisualizationTypeImageMapping
    {
        public static ObservableCollection<EventTypeSelectionVisualizationTypeImageMapping> AllEventTypeVisualizations = new ObservableCollection<EventTypeSelectionVisualizationTypeImageMapping>()
        {
            new EventTypeSelectionVisualizationTypeImageMapping(EventTypeSelectionVisualizationType.Organized, "/Images/Events/Settings/event_selector_radial_menu.png"),
            new EventTypeSelectionVisualizationTypeImageMapping(EventTypeSelectionVisualizationType.DropDown, "/Images/Events/Settings/event_selector_dropdown.png")
        };

        public EventTypeSelectionVisualizationTypeImageMapping(EventTypeSelectionVisualizationType type, string imgSource)
        {
            Type = type;
            ImageSource = imgSource;
        }

        public EventTypeSelectionVisualizationType Type { get; }

        public string ImageSource { get; }
    }
}
