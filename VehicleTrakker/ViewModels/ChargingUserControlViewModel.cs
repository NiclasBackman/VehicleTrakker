using NavigationTest;
using NavigationTest.ViewModels;
using System;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Services;

namespace VehicleTrakker.ViewModels
{
    class ChargingUserControlViewModel : BindableBase
    {
        private float energy;
        private string energyLabel;

        public ChargingUserControlViewModel() : base()
        {
            EnergyLabel = "Energy (" + SettingsService.Instance.QuerySettings().EnergyUnit + "):";
        }

        public string EnergyLabel
        {
            get { return energyLabel; }
            set
            {
                energyLabel = value;
                OnPropertyChanged("EnergyLabel");
            }
        }

        public float Energy
        {
            get { return energy; }
            set
            {
                energy = value;
                OnPropertyChanged("Energy");
            }
        }

        internal bool HasValidData()
        {
            return Energy > 0.0;
        }

        internal void ClearData()
        {
            Energy = 0.0F;
        }

        internal bool IsDirty(Guid eventId)
        {
            var evt = EventService.Instance.QueryEventById(eventId);
            if (evt == null)
            {
                return false;
            }
            var chargingEvent = evt as ChargingEvent;
            if (chargingEvent.Energy == Energy)
            {
                return false;
            }
            return true;
        }
    }

}
