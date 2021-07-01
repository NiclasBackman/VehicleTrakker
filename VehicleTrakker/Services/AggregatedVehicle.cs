using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;

namespace VehicleTrakker.Services
{
    public class AggregatedVehicle
    {
        public AggregatedVehicle()
        {
        }

        public AggregatedVehicle(Vehicle vehicle, List<Event> eventList)
        {
            Vehicle = vehicle;
            EventList = eventList;
        }

        public Vehicle Vehicle { get; set; }

        public List<Event> EventList { get; set; }
    }
}
