using NavigationTest;
using NavigationTest.ViewModels;
using System;
using VehicleTrakker.DataDefinitions;

namespace VehicleTrakker.ViewModels
{
    class RepairUserControlViewModel : BindableBase
    {
        private string repairShop;
        private EventService eventService;

        public RepairUserControlViewModel() : base()
        {
            eventService = EventService.Instance;
        }

        public string RepairShop
        {
            get
            {
                return repairShop;
            }
            set
            {
                repairShop = value;
                OnPropertyChanged("RepairShop");
            }
        }

        internal bool HasValidData()
        {
            return true;
        }

        internal void ClearData()
        {
            RepairShop = string.Empty;
        }

        internal bool IsDirty(Guid eventId)
        {
            var evt = eventService.QueryEventById(eventId) as RepairEvent;
            return evt.RepairShop != RepairShop;
        }
    }
}
