namespace VehicleTrakker.DataDefinitions
{
    public class WashEvent : Event
    {
        public WashEvent() : base()
        {
            this.Type = EventType.Wash;
        }
    }
}
