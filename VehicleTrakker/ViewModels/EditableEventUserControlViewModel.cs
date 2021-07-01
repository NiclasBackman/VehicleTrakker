using AsyncAwaitBestPractices.MVVM;
using NavigationTest;
using NavigationTest.UserControls;
using NavigationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.Services;
using VehicleTrakker.UserControls;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using static VehicleTrakker.DialogHelper;

namespace VehicleTrakker.ViewModels
{
    public class EditableEventUserControlViewModel : BindableBase
    {
        readonly Dictionary<EventType, UserControl> userControls;
        private Guid currentEventId;
        private UserControl selectedUserControl;
        private readonly DialogHelper dialogService;
        private readonly EventService eventService;
        private readonly VehicleService vehicleService;
        private readonly SettingsService settingsService;
        private readonly EditableEventUserControl parent;
        private DateTimeOffset eventDate;
        private int odometer;
        private float cost;
        private VehiclePersistenceState currentState;
        private string odometerUnit;
        private string costUnit;
        private string note;
        private Event currentEvent;
        private EventGeoPosition location;
        private MapStyle mapStyle;
        private MapProjection mapProjection;
        private Vehicle currentVehicle;

        public EditableEventUserControlViewModel(EditableEventUserControl parent)
        {
            dialogService = new DialogHelper();
            eventService = EventService.Instance;
            vehicleService = VehicleService.Instance;
            settingsService = SettingsService.Instance;
            settingsService.SettingsUpdatedObservable.Subscribe(HandleSettingsUpdated);
            eventService.AttachmentAddedObservable.Subscribe(HandleAttachmentAdded);
            eventService.AttachmentDeletedObservable.Subscribe(HandleAttachmentDeleted);
            eventService.EventUpdatedObservable.Subscribe(HandleEventUpdated);
            SaveEventCommand = new AsyncCommand(HandleEventSavedClickedAsync, CanSaveEvent);
            DeleteEventCommand = new AsyncCommand(HandleEventDeletedClicked, IsAlwaysTrue);
            AddAttachmentCommand = new AsyncCommand(HandleAttachmentAddedClicked, IsAlwaysTrue);
            ClearPositionCommand = new AsyncCommand(HandleClearPositionClicked, CanClearPosition);

            userControls = new Dictionary<EventType, UserControl>
            {
                [EventType.None] = new EmptyUserControl(),
                [EventType.Fuel] = new FuelUserControl(),
                [EventType.Charging] = new ChargingUserControl(),
                [EventType.Insurance] = new InsuranceUserControl(),
                [EventType.TollFee] = new TollFeeUserControl(),
                [EventType.Inspection] = new InspectionUserControl(),
                [EventType.Service] = new ServiceUserControl(),
                [EventType.Repair] = new RepairUserControl(),
                [EventType.Expense] = new ExpenseUserControl(),
                [EventType.Tax] = new TaxUserControl(),
                [EventType.Wash] = new WashUserControl(),
                [EventType.Action] = new ActionUserControl()
            };

            PropertyChangedEventHandler eh = new PropertyChangedEventHandler(ChildChanged);
            (userControls[EventType.Fuel].DataContext as FuelUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Charging].DataContext as ChargingUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Insurance].DataContext as InsuranceUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.TollFee].DataContext as TollFeeUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Inspection].DataContext as InspectionUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Service].DataContext as ServiceUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Repair].DataContext as RepairUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Expense].DataContext as ExpenseUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Tax].DataContext as TaxUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Wash].DataContext as WashUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Action].DataContext as ActionUserControlViewModel).PropertyChanged += eh;

            Attachments = new ObservableCollection<AttachmentViewModel>();
            var settings = settingsService.QuerySettings();
            OdometerUnit = "Odometer (" + settings.DistanceUnit + ")";
            var c = settings.CurrencyCultureCode;
            CostUnit = "Cost (" + settingsService.CurrencyCodeToCurrencySymbol(c) + ")";
            MapProjection = settings.SelectedMapProjection;
            MapStyle = settings.SelectedMapStyle;
            CurrentState = VehiclePersistenceState.Saved;
            this.parent = parent;
        }

        private void HandleEventUpdated(Guid eventId)
        {
            if(eventId == CurrentEventId)
            {
                SaveEventCommand.RaiseCanExecuteChanged();
            }
        }

        private void HandleSettingsUpdated(Settings settings)
        {
            OdometerUnit = "Odometer (" + settings.DistanceUnit + ")";
            CostUnit = "Cost (" + settingsService.CurrencyCodeToCurrencySymbol(settings.CurrencyCultureCode) + ")";
            MapStyle = settings.SelectedMapStyle;
            MapProjection = settings.SelectedMapProjection;
        }

        public IAsyncCommand SaveEventCommand { get; }

        public IAsyncCommand AddAttachmentCommand { get; }

        public IAsyncCommand DeleteEventCommand { get; }

        public IAsyncCommand ClearPositionCommand { get; }

        public ObservableCollection<AttachmentViewModel> Attachments { get; }

        public MapStyle MapStyle
        {
            get { return mapStyle; }
            set
            {
                mapStyle = value;
                OnPropertyChanged("MapStyle");
            }
        }

        public MapProjection MapProjection
        {
            get { return mapProjection; }
            set
            {
                mapProjection = value;
                OnPropertyChanged("MapProjection");
            }
        }

        public Guid CurrentEventId
        {
            get { return currentEventId; }
            set
            {
                currentEventId = value;
                OnPropertyChanged("CurrentEventId");
            }
        }

        public Event CurrentEvent
        {
            get { return currentEvent; }
            set
            {
                currentEvent = value;
                OnPropertyChanged("CurrentEvent");
            }
        }

        public DateTimeOffset EventDate
        {
            get { return eventDate; }
            set
            {
                eventDate = value;
                SaveEventCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("EventDate");
            }
        }

        public string Note
        {
            get { return note; }
            set
            {
                note = value;
                SaveEventCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("Note");
            }
        }

        public string CostUnit
        {
            get { return costUnit; }
            set
            {
                costUnit = value;
                OnPropertyChanged("CostUnit");
            }
        }

        public string OdometerUnit
        {
            get { return odometerUnit; }
            set
            {
                odometerUnit = value;
                OnPropertyChanged("OdometerUnit");
            }
        }

        public int Odometer
        {
            get { return odometer; }
            set
            {
                odometer = value;
                SaveEventCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("Odometer");
            }
        }

        public float Cost
        {
            get { return cost; }
            set
            {
                cost = value;
                SaveEventCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("Cost");
            }
        }

        public EventGeoPosition Location
        {
            get { return location; }
            set
            {
                location = value;
                SaveEventCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("Location");
            }
        }

        public UserControl CurrentUserControl
        {
            get { return selectedUserControl; }
            set
            {
                selectedUserControl = value;
                OnPropertyChanged("CurrentUserControl");
            }
        }

        public Vehicle CurrentVehicle
        {
            get { return currentVehicle; }
            set
            {
                currentVehicle = value;
                OnPropertyChanged("CurrentVehicle");
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

        private void ChildChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveEventCommand.RaiseCanExecuteChanged();
        }

        private bool IsAlwaysTrue(object arg)
        {
            return true;
        }

        private void HandleAttachmentAdded(Attachment attachment)
        {
            if(currentEventId == attachment.EventId)
            {
                var vm = new AttachmentViewModel
                {
                    FileName = attachment.FileName,
                    SourceFileName = attachment.SourceFileName,
                    Id = attachment.Id,
                    EventId = attachment.EventId
                };
                Attachments.Add(vm);
                OnPropertyChanged("Attachments");
            }
        }

        private void HandleAttachmentDeleted(Attachment attachment)
        {
            if (currentEventId == attachment.EventId)
            {
                var att = Attachments.Where(x => x.Id == attachment.Id).FirstOrDefault();
                if(att == null)
                {
                    throw new ArgumentException("Invalid attachment id handled");
                }

                Attachments.Remove(att);
            }
        }

        private async Task HandleAttachmentAddedClicked()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".doc");
            picker.FileTypeFilter.Add(".docx");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".pdf");
            picker.FileTypeFilter.Add(".xls");
            picker.FileTypeFilter.Add(".xlsx");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await eventService.AddAttachmentAsync(file, currentEventId);
            }
            else
            {
                return;
            }
        }

        private async Task HandleEventDeletedClicked()
        {
            var evt = eventService.QueryEventById(CurrentEvent.Id);
            var res = await dialogService.DisplayMessageQuestionAsync("Delete event of type [" + evt.Type + "]",
                                                          "You are about to delete the selected event, are you sure you want to proceed?");
            if(res == AnswerType.Yes)
            {
                await eventService.DeleteAsync(CurrentEvent.Id);
            }
        }

        private async Task HandleEventSavedClickedAsync()
        {
            var evt = (CurrentUserControl as IVehicleEventUserControl).GetData();
            evt.VehicleId = vehicleService.SelectedVehicleId;
            evt.Type = userControls.FirstOrDefault(x => x.Value == CurrentUserControl).Key;
            evt.Id = this.CurrentEventId;
            evt.Odometer = this.odometer;
            evt.TimeStamp = this.eventDate;
            evt.Cost = cost;
            evt.Note = note;
            evt.Location = this.Location;
            var attachments = new List<Attachment>();
            foreach(var vm in Attachments)
            {
                attachments.Add(new Attachment 
                {
                    EventId = vm.EventId,
                    Id = vm.Id,
                    FileName = vm.FileName,
                    SourceFileName = vm.SourceFileName
                });
            }
            evt.Attachments = attachments;
            await eventService.SaveAsync(evt);
        }

        private bool CanClearPosition(object arg)
        {
            return Location != null;
        }

        private Task HandleClearPositionClicked()
        {
            Location = null;
            parent.SetLocation(null);
            return Task.CompletedTask;
        }

        public bool CanSaveEvent(object arg)
        {
            if(CurrentUserControl == null)
            {
                return false;
            }

            var uc = (CurrentUserControl as IVehicleEventUserControl);

            return (IsDirty() || uc.IsDirty(CurrentEventId)) && HasValidData();
        }

        private bool HasValidData()
        {
            return (CurrentUserControl as IVehicleEventUserControl).HasValidData() && Odometer > 0;
        }

        private bool IsDirty()
        {
            var savedEvent = eventService.QueryEventById(CurrentEventId);
            if(savedEvent == null)
            {
                return false;
            }

            var ucIsDirty = (CurrentUserControl as IVehicleEventUserControl).IsDirty(CurrentEventId);

            if (savedEvent.TimeStamp == EventDate &&
                savedEvent.Odometer == Odometer &&
                savedEvent.Cost == Cost &&
                savedEvent.Note == Note &&
                savedEvent.Location?.Latitude == Location?.Latitude &&
                savedEvent.Location?.Longitude == Location?.Longitude && !ucIsDirty)
            {
                CurrentState = VehiclePersistenceState.Saved;
                return false;
            }
            CurrentState = VehiclePersistenceState.Edited;
            return true;
        }

        public void Populate(Guid eventId)
        {
            CurrentEventId = eventId;
            var evt = eventService.QueryEventById(eventId);
            if (evt == null)
            {
                CurrentUserControl = userControls[EventType.None];
            }
            else
            {
                CurrentVehicle = vehicleService.QueryVehicleById(evt.VehicleId);

                CurrentUserControl = userControls[evt.Type];
                (CurrentUserControl as IVehicleEventUserControl).Update(evt);

                EventDate = evt.TimeStamp;
                Odometer = evt.Odometer;
                Cost = evt.Cost;
                if (evt.Location != null)
                {
                    var basicGeoLoc = new BasicGeoposition { Latitude = evt.Location.Latitude, Longitude = evt.Location.Longitude };
                    parent.SetLocation(new Geopoint(basicGeoLoc));
                    Location = new EventGeoPosition(evt.Location.Latitude, evt.Location.Longitude);
                }
                else
                {
                    Location = null;
                }

                Attachments.Clear();
                foreach (var attachment in evt.Attachments)
                {
                    var vm = new AttachmentViewModel
                    {
                        FileName = attachment.FileName,
                        SourceFileName = attachment.SourceFileName,
                        Id = attachment.Id,
                        EventId = attachment.EventId
                    };
                    Attachments.Add(vm);
                    OnPropertyChanged("Attachments");
                }

                Note = evt.Note;
                CurrentEvent = evt;
                SaveEventCommand.RaiseCanExecuteChanged();
                CurrentState = VehiclePersistenceState.Saved;
            }
        }

        //private async Task HandleSelectedEventChangedAsync(Guid eventId)
        //{
        //    if(eventId == CurrentEventId)
        //    {
        //        return;
        //    }

        //    var uc = (CurrentUserControl as IVehicleEventUserControl);

        //    if (uc != null && (IsDirty() || uc.IsDirty(CurrentEventId)))
        //    {
        //        var res = await dialogService.DisplayMessageQuestionAsync("Navigate to new event",
        //                                                                  "You are about to discard changes in current event, are you sure you want to proceed?");
        //        if (res == AnswerType.No)
        //        {
        //            return;
        //        }
        //    }

        //    CurrentEventId = eventId;
        //    var evt = eventService.QueryEventById(eventId);
        //    if(evt == null)
        //    {
        //        CurrentUserControl = userControls[EventType.None];
        //    }
        //    else
        //    {
        //        CurrentUserControl = userControls[evt.Type];
        //        (CurrentUserControl as IVehicleEventUserControl).Update(evt);

        //        EventDate = evt.TimeStamp;
        //        Odometer = evt.Odometer;
        //        Cost = evt.Cost;
        //        Attachments.Clear();
        //        foreach(var attachment in evt.Attachments)
        //        {
        //            var vm = new AttachmentViewModel
        //            {
        //                FileName = attachment.FileName,
        //                SourceFileName = attachment.SourceFileName,
        //                Id = attachment.Id,
        //                EventId = attachment.EventId
        //            };
        //            Attachments.Add(vm);
        //        }

        //        Note = evt.Note;
        //        SaveEventCommand.RaiseCanExecuteChanged();
        //        CurrentState = VehiclePersistenceState.Saved;
        //    }
        //}
    }
}
