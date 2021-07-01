using System;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VehicleTrakker.UserControls
{
    public sealed partial class ServiceUserControl : UserControl, IVehicleEventUserControl
    {
        public ServiceUserControl()
        {
            this.InitializeComponent();
            this.DataContext = new ServiceUserControlViewModel();
        }

        public void ClearData()
        {
            (DataContext as ServiceUserControlViewModel).ClearData();
        }

        public Event GetData()
        {
            var vm = DataContext as ServiceUserControlViewModel;
            return new ServiceEvent(vm.ServiceStation);
        }

        public bool HasValidData()
        {
            return (DataContext as ServiceUserControlViewModel).HasValidData();
        }

        public bool IsDirty(Guid eventId)
        {
            return (DataContext as ServiceUserControlViewModel).IsDirty(eventId);
        }

        public void Update(Event evt)
        {
            var data = evt as ServiceEvent;
            (DataContext as ServiceUserControlViewModel).ServiceStation = data.ServiceStation;
        }
    }
}
