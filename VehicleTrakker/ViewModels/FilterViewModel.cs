using NavigationTest;
using NavigationTest.ViewModels;
using System;
using System.Linq;

namespace VehicleTrakker.ViewModels
{
    public class FilterViewModel : BindableBase
    {
        private bool filterIsEnabled;
        private DateTimeOffset startDate;
        private DateTimeOffset endDate;
        private VehicleService vehicleService;
        private EventService eventService;

        public FilterViewModel()
        {
            vehicleService = VehicleService.Instance;
            eventService = EventService.Instance;
            if(eventService.QueryAllEventsByVehicleId(vehicleService.SelectedVehicleId).Count > 0)
            {
                StartDate = eventService.QueryAllEventsByVehicleId(vehicleService.SelectedVehicleId).Min(e => e.TimeStamp);
                EndDate = eventService.QueryAllEventsByVehicleId(vehicleService.SelectedVehicleId).Max(e => e.TimeStamp).AddDays(1);
            }
            else
            {
                StartDate = DateTimeOffset.Now;
                EndDate = DateTimeOffset.Now.AddDays(1);
            }
        }

        public bool FilterIsEnabled
        {
            get { return filterIsEnabled; }
            set
            {
                filterIsEnabled = value;
                OnPropertyChanged("FilterIsEnabled");
            }
        }

        public DateTimeOffset StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        public DateTimeOffset EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                OnPropertyChanged("EndDate");
            }
        }
    }
}
