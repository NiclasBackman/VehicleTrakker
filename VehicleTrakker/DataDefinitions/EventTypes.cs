using System.ComponentModel;

namespace VehicleTrakker.DataDefinitions
{
    public enum EventType
    {
        [Description("None")]
        None = 0,
        [Description("Refueling")]
        Fuel,
        [Description("Insurance")]
        Insurance,
        [Description("Electric charging")]
        Charging,
        [Description("Scheduled service")]
        Service,
        [Description("Repair")]
        Repair,
        [Description("Expense")]
        Expense,
        [Description("Tax")]
        Tax,
        [Description("Toll road")]
        TollFee,
        [Description("Recurrent inspection")]
        Inspection,
        [Description("Wash")]
        Wash,
        [Description("Action")]
        Action
    }
}
