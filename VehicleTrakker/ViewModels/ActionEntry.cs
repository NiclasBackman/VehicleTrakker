using System.Collections.ObjectModel;

namespace VehicleTrakker.ViewModels
{
    public class ActionEntry
    {
        public static ObservableCollection<ActionEntry> AllActionTypes = new ObservableCollection<ActionEntry>()
        {
            new ActionEntry(ActionType.None, "/Images/Actions/exclamation.png"),
            new ActionEntry(ActionType.Acquired, "/Images/Actions/acquired.png"),
            new ActionEntry(ActionType.Deregister, "/Images/Actions/unregister.png"),
            new ActionEntry(ActionType.Register, "/Images/Actions/register.png"),
            new ActionEntry(ActionType.OwnerChange, "/Images/Actions/owner_change.png"),
            new ActionEntry(ActionType.Sold, "/Images/Actions/sold.png")
        };

        public ActionEntry(ActionType type, string imgSource)
        {
            Type = type;
            ImageSource = imgSource;
        }

        public ActionType Type { get; set; }

        public string ImageSource { get; set; }

        public string Description => Type.ToDescriptionString();
    }
}
