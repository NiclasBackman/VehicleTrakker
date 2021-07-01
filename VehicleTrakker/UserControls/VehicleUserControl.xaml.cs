using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VehicleTrakker.DataDefinitions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NavigationTest
{
    public sealed partial class VehicleUserControl : UserControl
    {
        public VehicleUserControl()
        {
            this.InitializeComponent();
        }

        public VehicleUserControl(Vehicle vehicle)
        {
            this.InitializeComponent();
            this.DataContext = new VehicleUserControlViewModel(vehicle, VehiclePersistenceState.Prepared);
        }
    }
}
