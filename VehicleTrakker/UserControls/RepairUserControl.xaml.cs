using System;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VehicleTrakker.UserControls
{
    public sealed partial class RepairUserControl : UserControl, IVehicleEventUserControl
    {
        public RepairUserControl()
        {
            this.InitializeComponent();
            this.DataContext = new RepairUserControlViewModel();
        }

        public void ClearData()
        {
            (DataContext as RepairUserControlViewModel).ClearData();
        }

        public Event GetData()
        {
            var vm = DataContext as RepairUserControlViewModel;
            return new RepairEvent(vm.RepairShop);
        }

        public bool HasValidData()
        {
            return (DataContext as RepairUserControlViewModel).HasValidData();
        }

        public bool IsDirty(Guid eventId)
        {
            return (DataContext as RepairUserControlViewModel).IsDirty(eventId);
        }

        public void Update(Event evt)
        {
            var data = evt as RepairEvent;
            (DataContext as RepairUserControlViewModel).RepairShop = data.RepairShop;
        }
    }
}
