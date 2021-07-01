using NavigationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.UserControls;
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

namespace NavigationTest.UserControls
{
    public sealed partial class FuelUserControl : UserControl, IVehicleEventUserControl
    {
        public FuelUserControl()
        {
            this.InitializeComponent();
            DataContext = new FuelUserControlViewModel();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var extension = new UserControlExtensions();
            extension.SetNumberBoxNumberFormatter(fuelQtyNumberBox, 0.01);
        }

        public void ClearData()
        {
            (DataContext as FuelUserControlViewModel).ClearData();
        }

        public Event GetData()
        {
            var qty = (DataContext as FuelUserControlViewModel).FuelQuantity;
            return new FuelEvent(qty);
        }

        public bool HasValidData()
        {
            return (DataContext as FuelUserControlViewModel).HasValidData();
        }

        public bool IsDirty(Guid eventId)
        {
            return (DataContext as FuelUserControlViewModel).IsDirty(eventId);
        }

        public void Update(Event evt)
        {
            var data = evt as FuelEvent;
            (DataContext as FuelUserControlViewModel).FuelQuantity = data.Volume;
        }
    }
}
