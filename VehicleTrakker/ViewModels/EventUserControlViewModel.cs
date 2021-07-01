using AsyncAwaitBestPractices.MVVM;
using NavigationTest.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Primitives.Menu;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Interfaces;
using VehicleTrakker.Services;
using VehicleTrakker.UserControls;
using VehicleTrakker.ViewModels;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using static VehicleTrakker.DataDefinitions.Settings;

namespace NavigationTest.ViewModels
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class EventUserControlViewModel : BindableBase
    {
        private EventImageMapping selectedEventType;
        private UserControl selectedUserControl;
        readonly Dictionary<EventType, UserControl> userControls;
        private int odometer;
        private VehiclePersistenceState currentState;
        private Guid eventId;
        private readonly EventService eventService;
        private readonly VehicleService vehicleService;
        private readonly SettingsService settingsService;
        private readonly EventUserControl parent;
        private DateTimeOffset eventDate;
        private float cost;
        private string costUnit;
        private string odometerUnit;
        private string note;
        private bool organizedEventPanelIsVisible;
        private MapStyle mapStyle;
        private MapProjection mapProjection;
        private EventGeoPosition location;

        public EventUserControlViewModel(EventUserControl parent) : base()
        {
            eventService = EventService.Instance;
            vehicleService = VehicleService.Instance;
            settingsService = SettingsService.Instance;
            var currentVehicle = vehicleService.QueryVehicleById(vehicleService.SelectedVehicleId);
            settingsService.SettingsUpdatedObservable.Subscribe(HandleSettingsUpdated);
            eventService.EventPreparedObservable.Subscribe(HandleEventPrepared);
            SaveEventCommand = new AsyncCommand(HandleEventSavedClicked, CanSaveEvent);
            DeleteEventCommand = new AsyncCommand(HandleEventDeletedClicked);
            ClearPositionCommand = new AsyncCommand(HandleClearPositionClicked, CanClearPosition);
            MyCustomCommand = new CustomItemCommand();
            EventTypes = EventImageMapping.QueryAllEvents(currentVehicle.EngineType);
            userControls = new Dictionary<EventType, UserControl>
            {
                [EventType.None] = new EmptyUserControl(),
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
            switch (currentVehicle.EngineType)
            {
                case EngineType.Hybrid:
                    userControls.Add(EventType.Fuel, new FuelUserControl());
                    (userControls[EventType.Fuel].DataContext as FuelUserControlViewModel).PropertyChanged += eh;
                    userControls.Add(EventType.Charging, new ChargingUserControl());
                    (userControls[EventType.Charging].DataContext as ChargingUserControlViewModel).PropertyChanged += eh;
                    break;
                case EngineType.ICE:
                    userControls.Add(EventType.Fuel, new FuelUserControl());
                    (userControls[EventType.Fuel].DataContext as FuelUserControlViewModel).PropertyChanged += eh;
                    break;
                case EngineType.PureEv:
                    userControls.Add(EventType.Charging, new ChargingUserControl());
                    (userControls[EventType.Charging].DataContext as ChargingUserControlViewModel).PropertyChanged += eh;
                    break;
            }

            SelectedEventType = EventTypes.First();

            (userControls[EventType.Insurance].DataContext as InsuranceUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.TollFee].DataContext as TollFeeUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Inspection].DataContext as InspectionUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Service].DataContext as ServiceUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Repair].DataContext as RepairUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Expense].DataContext as ExpenseUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Tax].DataContext as TaxUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Wash].DataContext as WashUserControlViewModel).PropertyChanged += eh;
            (userControls[EventType.Action].DataContext as ActionUserControlViewModel).PropertyChanged += eh;

            OdometerUnit = "Odometer (" + settingsService.QuerySettings().DistanceUnit + ")";
            var c = settingsService.QuerySettings().CurrencyCultureCode;
            CostUnit = "Cost (" + settingsService.CurrencyCodeToCurrencySymbol(c) + ")";

            var settings = settingsService.QuerySettings();
            OrganizedEventPanelIsVisible = settings.EventSelectionVisualization == EventTypeSelectionVisualizationType.Organized;
            MapProjection = settings.SelectedMapProjection;
            MapStyle = settings.SelectedMapStyle;
            CurrentState = VehiclePersistenceState.Saved;
            this.parent = parent;
        }


        private void HandleSettingsUpdated(Settings settings)
        {
            var state = CurrentState;
            OdometerUnit = "Odometer (" + settings.DistanceUnit + ")";
            CostUnit = "Cost (" + settingsService.CurrencyCodeToCurrencySymbol(settings.CurrencyCultureCode) + ")";
            OrganizedEventPanelIsVisible = settings.EventSelectionVisualization == EventTypeSelectionVisualizationType.Organized;
            MapStyle = settings.SelectedMapStyle;
            MapProjection = settings.SelectedMapProjection;
            CurrentState = state;
        }

        private void ChildChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName != "FuelQuantityLabel")
            {
                this.CurrentState = VehiclePersistenceState.Edited;
                SaveEventCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<EventImageMapping> EventTypes { get; }

        public ICommand MyCustomCommand { get; }
        
        public IAsyncCommand SaveEventCommand { get; }

        public IAsyncCommand DeleteEventCommand { get; }

        public IAsyncCommand ClearPositionCommand { get; }

        public bool OrganizedEventPanelIsVisible
        {
            get { return organizedEventPanelIsVisible; }
            set
            {
                organizedEventPanelIsVisible = value;
                OnPropertyChanged("OrganizedEventPanelIsVisible");
            }
        }

        public string Note
        {
            get { return note; }
            set
            {
                note = value;
                this.CurrentState = VehiclePersistenceState.Edited;
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

        public DateTimeOffset EventDate
        {
            get { return eventDate; }
            set
            {
                eventDate = value;
                this.CurrentState = VehiclePersistenceState.Edited;
                SaveEventCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("EventDate");
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

        public UserControl CurrentUserControl
        {
            get { return selectedUserControl; }
            set
            {
                selectedUserControl = value;
                OnPropertyChanged("CurrentUserControl");
            }
        }

        public EventImageMapping SelectedEventType
        {
            get { return selectedEventType; }
            set
            {
                selectedEventType = value;
                CurrentUserControl = userControls[selectedEventType.Type];
                SaveEventCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedEventType");
            }
        }

        public int Odometer
        {
            get { return odometer; }
            set
            {
                odometer = value;
                this.CurrentState = VehiclePersistenceState.Edited;
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
                this.CurrentState = VehiclePersistenceState.Edited;
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
                this.CurrentState = VehiclePersistenceState.Edited;
                SaveEventCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("Location");
            }
        }

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

        private Task HandleEventDeletedClicked()
        {
            eventId = Guid.Empty;
            Odometer = 0;
            Cost = 0.0f;
            Note = string.Empty;
            (CurrentUserControl as IVehicleEventUserControl).ClearData();
            SelectedEventType = EventTypes.First(); ;
            CurrentState = VehiclePersistenceState.Saved;
            eventService.CancelPrepareNewEvent();
            return Task.CompletedTask;
        }


        public bool CanSaveEvent(object arg)
        {
            return (CurrentUserControl as IVehicleEventUserControl).HasValidData() && Odometer > 0;
        }

        private async Task HandleEventSavedClicked()
        {
            var evt = (CurrentUserControl as IVehicleEventUserControl).GetData();
            evt.VehicleId = vehicleService.SelectedVehicleId;
            evt.Type = userControls.FirstOrDefault(x => x.Value == CurrentUserControl).Key;
            evt.Id = this.eventId;
            evt.Odometer = this.odometer;
            evt.TimeStamp = this.eventDate;
            evt.Cost = cost;
            evt.Note = this.note;
            evt.Location = this.location;
            await eventService.SaveAsync(evt);
        }

        private void HandleEventPrepared(Event eventCandidate)
        {
            this.eventId = eventCandidate.Id;
            Odometer = eventCandidate.Odometer;
            EventDate = eventCandidate.TimeStamp;
            Cost = eventCandidate.Cost;
            Note = string.Empty;
            Location = eventCandidate.Location;
            var basicGeoLoc = new BasicGeoposition { Latitude = eventCandidate.Location.Latitude, Longitude = eventCandidate.Location.Longitude };
            parent.SetLocation(new Geopoint(basicGeoLoc));
            (CurrentUserControl as IVehicleEventUserControl).ClearData();
            SelectedEventType = EventTypes.First();
            this.CurrentState = VehiclePersistenceState.Prepared;
            SaveEventCommand.RaiseCanExecuteChanged();
        }
    }
    public class CustomItemCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public CustomItemCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            // var item = parameter as RadialMenuItemContext;
            return true;
        }

        public void Execute(object parameter)
        {
            var context = parameter as RadialMenuItemContext;
            var target = context.TargetElement;
            var item = context.MenuItem;
            var commandParameter = context.CommandParameter;
            if(commandParameter == null)
            {
                return;
            }
            var type = (EventType)commandParameter;
            var ctx = (context.MenuItem.IconContent as Image).DataContext;
            var vm = ctx as EventUserControlViewModel;
            vm.SelectedEventType = vm.EventTypes.Where(x => x.Type == type).FirstOrDefault();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
