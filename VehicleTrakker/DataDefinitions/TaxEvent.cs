using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleTrakker.DataDefinitions
{
    public class TaxEvent : Event
    {
        public TaxEvent() : base()
        {
            this.Type = EventType.Tax;
        }
    }
}
