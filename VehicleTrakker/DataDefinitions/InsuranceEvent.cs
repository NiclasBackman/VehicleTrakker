using System;
using System.Xml.Serialization;
using VehicleTrakker.DataDefinitions;

namespace NavigationTest
{
    public class InsuranceEvent : Event
    {
        public InsuranceEvent() : base()
        {
            this.Type = EventType.Insurance;
        }

        public InsuranceEvent(DateTimeOffset startDate, DateTimeOffset endDate) : base()
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        [XmlIgnore]
        public DateTimeOffset StartDate { get; set; }

        [XmlElement("StartDateXml")]
        public string StartDateXml
        {
            get { return StartDate.ToString(); }
            set { StartDate = DateTimeOffset.Parse(value); }
        }

        [XmlIgnore]
        public DateTimeOffset EndDate { get; set; }

        [XmlElement("EndDateXml")]
        public string EndDateXml
        {
            get { return EndDate.ToString(); }
            set { EndDate = DateTimeOffset.Parse(value); }
        }

    }
}
