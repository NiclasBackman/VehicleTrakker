using System;
using System.Linq;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VehicleTrakker.UserControls
{
    public sealed partial class ActionUserControl : UserControl, IVehicleEventUserControl
    {
        public ActionUserControl()
        {
            this.InitializeComponent();
            DataContext = new ActionUserControlViewModel();
        }

        public void ClearData()
        {
            (DataContext as ActionUserControlViewModel).ClearData();
        }

        public Event GetData()
        {
            var actionType = (DataContext as ActionUserControlViewModel).SelectedAction.Type;
            return new ActionEvent(actionType);
        }

        public bool HasValidData()
        {
            return true;
        }

        public bool IsDirty(Guid eventId)
        {
            return (DataContext as ActionUserControlViewModel).IsDirty(eventId);
        }

        public void Update(Event evt)
        {
            var data = evt as ActionEvent;
            var vm = (DataContext as ActionUserControlViewModel);
            vm.SelectedAction = vm.ActionTypes.FirstOrDefault(x => x.Type == data.ActionType);
        }
    }
}
