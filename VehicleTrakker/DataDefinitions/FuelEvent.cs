using VehicleTrakker.DataDefinitions;

namespace NavigationTest
{
    public class FuelEvent : Event
    {
        public FuelEvent() : base()
        {
            this.Type = EventType.Fuel;
        }

        public FuelEvent(float volume) : this()
        {
            Volume = volume;
        }

        public float Volume { get; set; }
    }

    public class FuelEventExtended : FuelEvent
    {
        public FuelEventExtended(FuelEvent fuelEvent, float consumption) : base(fuelEvent.Volume)
        {
            this.Attachments = fuelEvent.Attachments;
            this.Cost = fuelEvent.Cost;
            this.Id = fuelEvent.Id;
            this.Note = fuelEvent.Note;
            this.Location = fuelEvent.Location;
            this.Odometer = fuelEvent.Odometer;
            this.TimeStamp = fuelEvent.TimeStamp;
            this.VehicleId = fuelEvent.VehicleId;
            this.Volume = fuelEvent.Volume;
            FuelConsumption = consumption;
        }

        public float FuelConsumption { get; set; }
    }
}
