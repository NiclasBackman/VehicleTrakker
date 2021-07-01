namespace VehicleTrakker.DataDefinitions
{
    public class RepairEvent : Event
    {
        public RepairEvent() : base()
        {
            this.Type = EventType.Repair;
        }

        public RepairEvent(string repairShop) : base()
        {
            RepairShop = repairShop ?? string.Empty;
        }

        public string RepairShop { get; set; }
    }
}
