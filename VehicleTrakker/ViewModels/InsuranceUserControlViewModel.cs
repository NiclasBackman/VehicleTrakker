using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigationTest.ViewModels
{
    class InsuranceUserControlViewModel : BindableBase
    {
        private DateTimeOffset startDate;
        private DateTimeOffset endDate;

        public InsuranceUserControlViewModel() : base()
        {
            StartDate = DateTimeOffset.Now;
            EndDate = DateTimeOffset.Now.AddYears(1);
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

        internal bool HasValidData()
        {
            bool res = StartDate < EndDate;
            return res;
        }

        internal void ClearData()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
        }

        internal bool IsDirty(Guid eventId)
        {
            var evt = EventService.Instance.QueryEventById(eventId);
            if(evt == null)
            {
                return false;
            }
            var insuranceEvent = evt as InsuranceEvent;
            if(insuranceEvent.StartDate == StartDate &&
               insuranceEvent.EndDate == EndDate)
            {
                return false;
            }
            return true;
        }
    }
}
