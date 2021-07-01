using AsyncAwaitBestPractices.MVVM;
using NavigationTest;
using NavigationTest.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Services;
using static VehicleTrakker.DataDefinitions.Reminder;
using static VehicleTrakker.DialogHelper;

namespace VehicleTrakker.ViewModels
{
    public class ReminderViewModel : BindableBase
    {
        private DateTimeOffset creationDate;
        private ReminderState state;
        private DateTimeOffset expirationDate;
        private string message;
        private DialogHelper dialogService;
        private ReminderService reminderService;
        private VehicleService vehicleService;
        private VehiclePersistenceState currentState;
        private ObservableCollection<Vehicle> allVehicles;
        private Vehicle selectedVehicle;

        public ReminderViewModel(Reminder reminder) : base()
        {
            dialogService = new DialogHelper();
            reminderService = ReminderService.Instance;
            vehicleService = VehicleService.Instance;
            reminderService.ReminderUpdatedObservable.Subscribe(HandleReminderUpdated);
            vehicleService.VehicleUpdatedObservable.Subscribe(HandleVehicleUpdated);
            vehicleService.VehicleCreatedObservable.Subscribe(HandleVehicleCreated);
            vehicleService.VehicleDeletedObservable.Subscribe(HandleVehicleDeleted);

            SaveReminderCommand = new AsyncCommand(HandleSaveReminderClicked, CanSaveReminder);
            DeleteReminderCommand = new AsyncCommand(HandleDeleteReminderClicked, CanDeleteReminder);
            Id = reminder.Id;
            VehicleId = reminder.VehicleId;
            ExpirationDate = reminder.ExpirationDate;
            Message = reminder.Message;
            CreationDate = reminder.CreationDate;
            State = reminder.State;
            AllVehicles = new ObservableCollection<Vehicle>();
            PopulateVehicles();
            CurrentState = VehiclePersistenceState.Saved;
        }

        private void PopulateVehicles()
        {
            AllVehicles.Clear();
            vehicleService.QueryAllVehicles().ForEach(x => AllVehicles.Add(x));
            AllVehicles.Insert(0, new Vehicle()
            {
                Name = "-",
                Id = Guid.Empty
            });

            SelectedVehicle = AllVehicles.Where(x => x.Id == VehicleId).FirstOrDefault();
        }

        public IAsyncCommand SaveReminderCommand { get; }

        public IAsyncCommand DeleteReminderCommand { get; }

        public Guid Id { get; set; }

        public Guid VehicleId { get; set; }

        public ObservableCollection<Vehicle> AllVehicles
        {
            get { return allVehicles; }
            set
            {
                allVehicles = value;
                OnPropertyChanged("AllVehicles");
            }
        }

        public Vehicle SelectedVehicle
        {
            get { return selectedVehicle; }
            set
            {
                selectedVehicle = value;
                OnPropertyChanged("SelectedVehicle");
                SaveReminderCommand.RaiseCanExecuteChanged();
            }
        }

        public VehiclePersistenceState CurrentState
        {
            get { return currentState; }
            set
            {
                currentState = value;
                OnPropertyChanged("CurrentState");
            }
        }

        public ReminderState State 
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

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

        public DateTimeOffset ExpirationDate
        {
            get
            {
                return expirationDate;
            }
            set
            {
                expirationDate = value;
                OnPropertyChanged("ExpirationDate");
                SaveReminderCommand.RaiseCanExecuteChanged();
            }
        }

        public string Message 
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                OnPropertyChanged("Message");
                SaveReminderCommand.RaiseCanExecuteChanged();
            }
        }

        private Reminder ToReminder()
        {
            return new Reminder
            {
                Id = this.Id,
                ExpirationDate = this.ExpirationDate,
                Message = this.Message,
                CreationDate = this.CreationDate,
                State = this.State,
                VehicleId = this.SelectedVehicle.Id
            };
        }

        private async Task HandleSaveReminderClicked()
        {
            await reminderService.UpdateAsync(ToReminder());
        }

        private bool CanSaveReminder(object arg)
        {
            var isDirty = IsDirty();
            CurrentState = isDirty ? VehiclePersistenceState.Edited : VehiclePersistenceState.Saved;
            return isDirty && (ExpirationDate > CreationDate) && !string.IsNullOrEmpty(Message);
        }

        private async Task HandleDeleteReminderClicked()
        {
            var res = await dialogService.DisplayMessageQuestionAsync("Delete reminder that expires "+ ExpirationDate.DateTime.ToShortDateString(),
                                  "You are about to delete the reminder, are you sure you want to proceed?");
            if (res == AnswerType.Yes)
            {
                await reminderService.DeleteAsync(Id);
            }
        }

        private bool CanDeleteReminder(object arg)
        {
            return true;
        }

        private bool IsDirty()
        {
            var persistedReminder = reminderService.QueryReminderById(Id);
            if(persistedReminder == null)
            {
                return false;
            }

            if (persistedReminder.ExpirationDate == ExpirationDate &&
               persistedReminder.CreationDate == CreationDate &&
               persistedReminder.Message == Message &&
               persistedReminder.VehicleId == SelectedVehicle.Id)
            {
                return false;
            }
            return true;
        }

        private void HandleReminderUpdated(Guid reminderId)
        {
            if(this.Id == reminderId)
            {
                var reminder = reminderService.QueryReminderById(Id);
                ExpirationDate = reminder.ExpirationDate;
                SaveReminderCommand.RaiseCanExecuteChanged();
            }
        }

        private void HandleVehicleCreated(Guid vehicleId)
        {
            PopulateVehicles();
        }

        private void HandleVehicleDeleted(Guid vehicleId)
        {
            PopulateVehicles();
        }

        private void HandleVehicleUpdated(Guid vehicleId)
        {
            PopulateVehicles();
        }
    }
}
