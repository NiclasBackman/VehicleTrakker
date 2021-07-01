using NavigationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.ViewModels;
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

namespace VehicleTrakker.UserControls
{
    public sealed partial class TollFeeUserControl : UserControl, IVehicleEventUserControl
    {
        public TollFeeUserControl()
        {
            this.InitializeComponent();
            this.DataContext = new TollFeeUserControlViewModel();
        }

        public void ClearData()
        {
            (DataContext as TollFeeUserControlViewModel).ClearData();
        }

        public Event GetData()
        {
            var vm = DataContext as TollFeeUserControlViewModel;
            return new TollFeeEvent();
        }

        public bool HasValidData()
        {
            return (DataContext as TollFeeUserControlViewModel).HasValidData();
        }

        public bool IsDirty(Guid eventId)
        {
            return (DataContext as TollFeeUserControlViewModel).IsDirty(eventId);
        }

        public void Update(Event evt)
        {
        }
    }
}
