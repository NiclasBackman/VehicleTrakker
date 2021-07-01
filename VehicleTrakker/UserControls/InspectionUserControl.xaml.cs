using System;
using System.Linq;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VehicleTrakker.UserControls
{
    public sealed partial class InspectionUserControl : UserControl, IVehicleEventUserControl
    {
        public InspectionUserControl()
        {
            this.InitializeComponent();
            DataContext = new InspectionUserControlViewModel();
        }

        public void ClearData()
        {
            (DataContext as InspectionUserControlViewModel).ClearData();
        }

        public Event GetData()
        {
            var vm = DataContext as InspectionUserControlViewModel;
            return new InspectionEvent(vm.SelectedResult.Result);
        }

        public bool HasValidData()
        {
            return (DataContext as InspectionUserControlViewModel).HasValidData();
        }

        public bool IsDirty(Guid eventId)
        {
            return (DataContext as InspectionUserControlViewModel).IsDirty(eventId);
        }

        public void Update(Event evt)
        {
            var data = evt as InspectionEvent;
            var vm = (DataContext as InspectionUserControlViewModel);
            vm.SelectedResult = vm.InspectionResults.Where(x => x.Result == data.Result).FirstOrDefault();
        }
    }
}
