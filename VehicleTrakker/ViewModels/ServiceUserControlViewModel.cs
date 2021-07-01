using NavigationTest;
using NavigationTest.ViewModels;
using System;
using VehicleTrakker.DataDefinitions;

namespace VehicleTrakker.ViewModels
{
    class ServiceUserControlViewModel : BindableBase
    {
        private string serviceStation;
        private EventService eventService;

        public ServiceUserControlViewModel() : base()
        {
            eventService = EventService.Instance;
        }

        public string ServiceStation
        {
            get
            {
                return serviceStation;
            }
            set
            {
                serviceStation = value;
                OnPropertyChanged("ServiceStation");
            }
        }

        internal bool HasValidData()
        {
            return true;
        }

        internal void ClearData()
        {
            ServiceStation = string.Empty;
        }

        internal bool IsDirty(Guid eventId)
        {
            var evt = eventService.QueryEventById(eventId) as ServiceEvent;
            return evt.ServiceStation != ServiceStation;
        }
    }
}
