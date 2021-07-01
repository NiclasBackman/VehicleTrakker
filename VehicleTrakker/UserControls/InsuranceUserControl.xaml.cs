using NavigationTest.ViewModels;
using System;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NavigationTest.UserControls
{
    public sealed partial class InsuranceUserControl : UserControl, IVehicleEventUserControl
    {
        public InsuranceUserControl()
        {
            this.InitializeComponent();
            DataContext = new InsuranceUserControlViewModel();
        }

        public void ClearData()
        {
            (DataContext as InsuranceUserControlViewModel).ClearData();
        }

        public Event GetData()
        {
            var vm = DataContext as InsuranceUserControlViewModel;
            return new InsuranceEvent(vm.StartDate, vm.EndDate);
        }

        public bool HasValidData()
        {
            return (DataContext as InsuranceUserControlViewModel).HasValidData();
        }

        public bool IsDirty(Guid eventId)
        {
            return (DataContext as InsuranceUserControlViewModel).IsDirty(eventId);
        }

        public void Update(Event evt)
        {
            var data = evt as InsuranceEvent;
            (DataContext as InsuranceUserControlViewModel).StartDate = data.StartDate;
            (DataContext as InsuranceUserControlViewModel).EndDate = data.EndDate;
        }
    }
}
