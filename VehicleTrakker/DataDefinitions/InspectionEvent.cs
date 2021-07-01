using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.ViewModels;

namespace VehicleTrakker.DataDefinitions
{
    public class InspectionEvent : Event
    {
        public InspectionEvent() : base()
        {
            this.Type = EventType.Inspection;
        }

        public InspectionEvent(InspectionResultType result) : this()
        {
            Result = result;
        }

        public InspectionResultType Result { get; set; }

    }
}
