namespace VehicleTrakker.DataDefinitions
{
    public class ServiceEvent : Event
    {
        public ServiceEvent() : base()
        {
            this.Type = EventType.Service;
        }

        public ServiceEvent(string serviceStation) : this()
        {
            ServiceStation = serviceStation ?? string.Empty;
        }

        public string ServiceStation { get; set; }
    }
}
