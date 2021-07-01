using NavigationTest;
using NavigationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Windows.Devices.Geolocation;

namespace VehicleTrakker.DataDefinitions
{
    [XmlInclude(typeof(InspectionEvent)), XmlInclude(typeof(FuelEvent)),
     XmlInclude(typeof(ChargingEvent)),   XmlInclude(typeof(ExpenseEvent)),
     XmlInclude(typeof(InsuranceEvent)),  XmlInclude(typeof(RepairEvent)),
     XmlInclude(typeof(ServiceEvent)),    XmlInclude(typeof(TaxEvent)),
     XmlInclude(typeof(TollFeeEvent)),    XmlInclude(typeof(WashEvent))]
    public class Event
    {
        public Event()
        {
            Attachments = new List<Attachment>();
        }

        public Guid VehicleId { get; set; }

        public Guid Id { get; set; }

        public EventType Type { get; set; }

        [XmlIgnore]
        public DateTimeOffset TimeStamp { get; set; } 

        [XmlElement("TimeStampXml")]
        public string TimeStampXml
        {
            get { return TimeStamp.ToString(); }
            set { TimeStamp = DateTimeOffset.Parse(value); }
        }

        public int Odometer { get; set; }

        public float Cost { get; set; }

        public string Note { get; set; }

        public EventGeoPosition Location { get; set; }

        public List<Attachment> Attachments { get; set; }
    }
}
