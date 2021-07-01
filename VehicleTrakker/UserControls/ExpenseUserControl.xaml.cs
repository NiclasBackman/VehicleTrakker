using System;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VehicleTrakker.UserControls
{
    public sealed partial class ExpenseUserControl : UserControl, IVehicleEventUserControl
    {
        public ExpenseUserControl()
        {
            this.InitializeComponent();
            DataContext = new ExpenseUserControlViewModel();
        }

        public void ClearData()
        {
            (DataContext as ExpenseUserControlViewModel).ClearData();
        }

        public Event GetData()
        {
            var vm = DataContext as ExpenseUserControlViewModel;
            return new ExpenseEvent();
        }

        public bool HasValidData()
        {
            return (DataContext as ExpenseUserControlViewModel).HasValidData();
        }

        public bool IsDirty(Guid eventId)
        {
            return (DataContext as ExpenseUserControlViewModel).IsDirty(eventId);
        }

        public void Update(Event evt)
        {
        }
    }
}
