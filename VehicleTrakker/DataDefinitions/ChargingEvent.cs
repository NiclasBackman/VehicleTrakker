namespace VehicleTrakker.DataDefinitions
{
    public class ChargingEvent : Event
    {
        public ChargingEvent() : base()
        {
            this.Type = EventType.Charging;
        }

        public ChargingEvent(float energy) : this()
        {
            Energy = energy;
        }

        public float Energy { get; set; }
    }

    public class ChargingEventExtended : ChargingEvent
    {
        public ChargingEventExtended(ChargingEvent chargingEvent, float consumption) : base(chargingEvent.Energy)
        {
            this.Attachments = chargingEvent.Attachments;
            this.Cost = chargingEvent.Cost;
            this.Id = chargingEvent.Id;
            this.Note = chargingEvent.Note;
            this.Location = chargingEvent.Location;
            this.Odometer = chargingEvent.Odometer;
            this.TimeStamp = chargingEvent.TimeStamp;
            this.VehicleId = chargingEvent.VehicleId;
            EnergyConsumption = consumption;
        }

        public float EnergyConsumption { get; set; }
    }

}
