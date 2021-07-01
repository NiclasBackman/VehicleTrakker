using NavigationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleTrakker.DataDefinitions
{
    public class Reminder : BindableBase
    {
        private DateTimeOffset creationDate;

        public enum ReminderState
        {
            Idle = 0,
            Expired,
            Confirmed
        }

        public Reminder()
        {
        }

        public Reminder(DateTimeOffset expirationDate, string message, Guid vehicleId) : base()
        {
            Id = Guid.NewGuid();
            ExpirationDate = expirationDate;
            Message = message;
            CreationDate = DateTimeOffset.Now;
            State = ReminderState.Idle;
            VehicleId = vehicleId;
        }

        public Guid Id { get; set; }

        public Guid VehicleId { get; set; }

        public ReminderState State { get; set; }

        public DateTimeOffset CreationDate
        { 
            get
            {
                return creationDate;
            }
            set
            {
                creationDate = value;
                OnPropertyChanged("CreationDate");
            }
        }

        public DateTimeOffset ExpirationDate { get; set; }

        public string Message { get; set; }
    }
}
