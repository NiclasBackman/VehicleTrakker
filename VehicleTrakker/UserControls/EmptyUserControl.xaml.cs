using System;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NavigationTest.UserControls
{
    public sealed partial class EmptyUserControl : UserControl, IVehicleEventUserControl
    {
        public EmptyUserControl()
        {
            this.InitializeComponent();
        }

        public void ClearData()
        {
        }

        public Event GetData()
        {
            return null;
        }

        public bool HasValidData()
        {
            return false;
        }

        public bool IsDirty(Guid eventId)
        {
            return false;
        }

        public void Update(Event evt)
        {
        }
    }
}
