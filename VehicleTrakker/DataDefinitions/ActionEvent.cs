using VehicleTrakker.ViewModels;

namespace VehicleTrakker.DataDefinitions
{
    public class ActionEvent : Event
    {
        public ActionEvent() : base()
        {
            this.Type = EventType.Action;
        }

        public ActionEvent(ActionType actionType) : this()
        {
            ActionType = actionType;
        }

        public ActionType ActionType { get; set; }
    }
}
