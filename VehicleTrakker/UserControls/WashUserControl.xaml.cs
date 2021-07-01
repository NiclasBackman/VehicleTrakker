using System;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VehicleTrakker.UserControls
{
    public sealed partial class WashUserControl : UserControl, IVehicleEventUserControl
    {
        public WashUserControl()
        {
            this.InitializeComponent();
            DataContext = new WashUserControlViewModel();
        }

        public void ClearData()
        {
            (DataContext as WashUserControlViewModel).ClearData();
        }

        public Event GetData()
        {
            var vm = DataContext as WashUserControlViewModel;
            return new WashEvent();
        }

        public bool HasValidData()
        {
            return (DataContext as WashUserControlViewModel).HasValidData();
        }

        public bool IsDirty(Guid eventId)
        {
            return (DataContext as WashUserControlViewModel).IsDirty(eventId);
        }

        public void Update(Event evt)
        {
        }
    }
}
