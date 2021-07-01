using System;
using System.Collections.ObjectModel;
using System.Linq;
using VehicleTrakker.DataDefinitions;

namespace NavigationTest
{
    public class EventImageMapping
    {
        public static ObservableCollection<EventImageMapping> AllEvents = new ObservableCollection<EventImageMapping>()
        {
            new EventImageMapping(EventType.None, "/Images/Events/blocked.png"),
            new EventImageMapping(EventType.Fuel, "/Images/Events/gas_station.png"),
            new EventImageMapping(EventType.Charging, "/Images/Events/charging_station.png"),
            new EventImageMapping(EventType.Insurance, "/Images/Events/insurance.png"),
            new EventImageMapping(EventType.TollFee, "/Images/Events/toll_fee.png"),
            new EventImageMapping(EventType.Inspection, "/Images/Events/vehicle_inspection.png"),
            new EventImageMapping(EventType.Service, "/Images/Events/service.png"),
            new EventImageMapping(EventType.Repair, "/Images/Events/repair.png"),
            new EventImageMapping(EventType.Expense, "/Images/Events/expenses.png"),
            new EventImageMapping(EventType.Tax, "/Images/Events/tax.png"),
            new EventImageMapping(EventType.Wash, "/Images/Events/wash.png"),
            new EventImageMapping(EventType.Action, "/Images/Events/action.png")
        };

        public EventImageMapping(EventType type, string imgSource)
        {
            Type = type;
            ImageSource = imgSource;
        }

        public EventType Type { get; }

        public string ImageSource { get; }

        internal static ObservableCollection<EventImageMapping> QueryAllEvents(EngineType engineType)
        {
            switch(engineType)
            {
                case EngineType.ICE:
                    return new ObservableCollection<EventImageMapping>(AllEvents.Where(x => x.Type != EventType.Charging).ToList());
                case EngineType.Hybrid:
                    return AllEvents;
                case EngineType.PureEv:
                    return new ObservableCollection<EventImageMapping>(AllEvents.Where(x => x.Type != EventType.Fuel).ToList());
                default:
                    return null;
            }
        }
    }
}
