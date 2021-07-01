using NavigationTest;
using NavigationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;

namespace VehicleTrakker.ViewModels
{
    public enum ActionType
    {
        [Description("Unspecified")]
        None,
        [Description("De-register")]
        Deregister,
        [Description("Register")]
        Register,
        [Description("Sold")]
        Sold,
        [Description("Acquired")]
        Acquired,
        [Description("Change of owner")]
        OwnerChange
    };

    public class ActionUserControlViewModel : BindableBase
    {
        private ActionEntry selectedAction;

        public ActionUserControlViewModel()
        {
            ActionTypes = ActionEntry.AllActionTypes;
            SelectedAction = ActionTypes.First();
        }

        public ObservableCollection<ActionEntry> ActionTypes { get; set; }


        public ActionEntry SelectedAction
        {
            get { return selectedAction; }
            set
            {
                selectedAction = value;
                OnPropertyChanged("SelectedAction");
            }
        }

        public void ClearData()
        {
            SelectedAction = ActionTypes.Where(x => x.Type == ActionType.None).FirstOrDefault();
        }
        public bool IsDirty(Guid eventId)
        {
            var evt = EventService.Instance.QueryEventById(eventId);
            if (evt == null)
            {
                return false;
            }
            var actionEvent = evt as ActionEvent;
            if (actionEvent.ActionType == SelectedAction.Type)
            {
                return false;
            }
            return true;
        }
    }
}
