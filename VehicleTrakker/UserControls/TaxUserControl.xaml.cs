using System;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VehicleTrakker.UserControls
{
    public sealed partial class TaxUserControl : UserControl, IVehicleEventUserControl
    {
        public TaxUserControl()
        {
            this.InitializeComponent();
            DataContext = new TaxUserControlViewModel();
        }

        public void ClearData()
        {
            (DataContext as TaxUserControlViewModel).ClearData();
        }

        public Event GetData()
        {
            var vm = DataContext as TaxUserControlViewModel;
            return new TaxEvent();
        }

        public bool HasValidData()
        {
            return (DataContext as TaxUserControlViewModel).HasValidData();
        }

        public bool IsDirty(Guid eventId)
        {
            return (DataContext as TaxUserControlViewModel).IsDirty(eventId);
        }

        public void Update(Event evt)
        {
        }
    }
}
