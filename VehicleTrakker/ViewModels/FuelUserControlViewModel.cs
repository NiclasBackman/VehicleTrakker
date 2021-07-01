using System;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Services;

namespace NavigationTest.ViewModels
{
    class FuelUserControlViewModel : BindableBase
    {
        private float fuelQuantity;
        private string fuelQuantityLabel;
        private SettingsService settingsService;

        public FuelUserControlViewModel() : base()
        {
            settingsService = SettingsService.Instance;
            FuelQuantityLabel = "Volume (" + settingsService.QuerySettings().VolumeUnit + ")";
            settingsService.SettingsUpdatedObservable.Subscribe(HandleSettingsUpdated);
        }

        private void HandleSettingsUpdated(Settings settings)
        {
            FuelQuantityLabel = "Volume (" + settings.VolumeUnit + ")";
        }

        public string FuelQuantityLabel
        {
            get { return fuelQuantityLabel; }
            set
            {
                fuelQuantityLabel = value;
                OnPropertyChanged("FuelQuantityLabel");
            }
        }

        public float FuelQuantity
        {
            get { return fuelQuantity; }
            set
            {
                fuelQuantity = value;
                OnPropertyChanged("FuelQuantity");
            }
        }

        internal bool HasValidData()
        {
            var parseRes = FuelQuantity;
            return FuelQuantity > 0.0;
        }

        internal void ClearData()
        {
            FuelQuantity = 0.0F;
        }

        internal bool IsDirty(Guid eventId)
        {
            var evt = EventService.Instance.QueryEventById(eventId);
            if (evt == null)
            {
                return false;
            }
            var fuelEvent = evt as FuelEvent;
            if (fuelEvent.Volume == FuelQuantity)
            {
                return false;
            }
            return true;
        }
    }
}
