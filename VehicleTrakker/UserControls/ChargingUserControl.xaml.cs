using System;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VehicleTrakker.UserControls
{
    public sealed partial class ChargingUserControl : UserControl, IVehicleEventUserControl
    {
        public ChargingUserControl()
        {
            this.InitializeComponent();
            DataContext = new ChargingUserControlViewModel();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var extension = new UserControlExtensions();
            extension.SetNumberBoxNumberFormatter(energyNumberBox, 0.01);
        }

        public void ClearData()
        {
            (DataContext as ChargingUserControlViewModel).ClearData();
        }

        public Event GetData()
        {
            var energy = (DataContext as ChargingUserControlViewModel).Energy;
            return new ChargingEvent(energy);
        }

        public bool HasValidData()
        {
            return (DataContext as ChargingUserControlViewModel).HasValidData();
        }

        public bool IsDirty(Guid eventId)
        {
            return (DataContext as ChargingUserControlViewModel).IsDirty(eventId);
        }

        public void Update(Event evt)
        {
            var data = evt as ChargingEvent;
            (DataContext as ChargingUserControlViewModel).Energy = data.Energy;
        }
    }
}
