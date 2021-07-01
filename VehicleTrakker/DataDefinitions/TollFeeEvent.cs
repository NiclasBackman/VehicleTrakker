namespace VehicleTrakker.DataDefinitions
{
    public class TollFeeEvent : Event
    {
        public TollFeeEvent() : base()
        {
            this.Type = EventType.TollFee;
        }
    }
}
