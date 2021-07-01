using AsyncAwaitBestPractices.MVVM;
using Microsoft.Toolkit.Uwp.UI;
using NavigationTest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Pages;
using VehicleTrakker.Services;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using static VehicleTrakker.DialogHelper;

namespace VehicleTrakker.ViewModels
{

    class EventsPageViewModel : INotifyPropertyChanged
    {
        private readonly VehicleService vehicleService;
        private readonly EventService eventService;
        private readonly SettingsService settingsService;
        private readonly DialogHelper dialogService;
        private readonly AdvancedCollectionView cvs;
        private Vehicle selectedVehicle;
        private Event selectedVehicleEvent;
        private readonly ObservableCollection<Event> vehicleEvents;
        private readonly FilterViewModel filterViewModel;
        private readonly Page parent;

        public EventsPageViewModel(Page parent)
        {
            vehicleService = VehicleService.Instance;
            eventService = EventService.Instance;
            settingsService = SettingsService.Instance;
            dialogService = new DialogHelper();

            Vehicles = new ObservableCollection<Vehicle>();
            vehicleEvents = new ObservableCollection<Event>();
            cvs = new AdvancedCollectionView(vehicleEvents, true);
            cvs.Filter = null; // No filter applied initially
            cvs.SortDescriptions.Add(new SortDescription(nameof(Event.Odometer), SortDirection.Descending));
            AddEventCommand = new RelayCommand(HandleAddEventClicked, CanAddEvent);
            EditEventCommand = new AsyncCommand<Guid>(HandleEditEventClicked);
            DeleteEventCommand = new AsyncCommand<Guid>(HandleEventDeletedClicked);

            foreach (var vehicle in vehicleService.QueryAllVehicles())
            {
                Vehicles.Add(vehicle);
            }

            SelectedVehicle = Vehicles.Where(x => x.IsFavorite).FirstOrDefault();
            settingsService.SettingsUpdatedObservable.Subscribe(HandleSettingsUpdated);
            eventService.EventCreatedObservable.Subscribe(HandleEventCreated);
            eventService.EventUpdatedObservable.Subscribe(HandleEventUpdated);
            eventService.EventDeletedObservable.Subscribe(HandleEventDeleted);
            eventService.AttachmentAddedObservable.Subscribe(HandleAttachmentAdded);
            eventService.AttachmentDeletedObservable.Subscribe(HandleAttachmentDeleted);
            vehicleService.SelectedVehicleChangedObservable.Subscribe(HandleSelectedVehicleChanged);
            vehicleService.VehicleDeletedObservable.Subscribe(HandleVehicleDeleted);
            vehicleService.VehicleCreatedObservable.Subscribe(HandleVehicleCreated);
            vehicleService.VehicleUpdatedObservable.Subscribe(HandleVehicleUpdated);
            if (SelectedVehicle != null)
            {
                Populate(SelectedVehicle.Id);
            }

            filterViewModel = new FilterViewModel();
            filterViewModel.PropertyChanged += new PropertyChangedEventHandler(FilterChanged);
            this.parent = parent;
        }

        private void Populate(Guid vehicleId)
        {
            var allEvents = eventService.QueryAllEventsByVehicleId(vehicleId);
            foreach (var evt in allEvents)
            {
                if (evt.Type == EventType.Fuel)
                {
                    var fEvt = new FuelEventExtended(evt as FuelEvent, CalculateFuelConsumption(evt, allEvents));
                    vehicleEvents.Add(fEvt);
                }
                else if (evt.Type == EventType.Charging)
                {
                    var cEvt = new ChargingEventExtended(evt as ChargingEvent, CalculateEnergyConsumption(evt, allEvents));
                    vehicleEvents.Add(cEvt);
                }
                else
                {
                    vehicleEvents.Add(evt);
                }
            }
        }

        private void FilterChanged(object sender, PropertyChangedEventArgs e)
        {
            var filterVm = sender as FilterViewModel;
            if(filterViewModel.FilterIsEnabled == true)
            {
                cvs.Filter = null;
                cvs.Filter = new Predicate<object>(Do_Filtering);
            }
            else
            {
                cvs.Filter = null;
            }
        }

        public FilterViewModel Filter => filterViewModel;

        private bool Do_Filtering(object obj)
        {
            var evt = obj as Event;
            if(evt.TimeStamp >= filterViewModel.StartDate && evt.TimeStamp <= filterViewModel.EndDate)
            {
                return true;
            }
            return false;
        }

        private float CalculateEnergyConsumption(Event evt, List<Event> events)
        {
            var fuelEvents = events.Where(x => x.Type == EventType.Charging).ToList().OrderBy(y => y.Odometer).Reverse();
            var previous = fuelEvents.FirstOrDefault(x => x.Odometer < evt.Odometer);
            if (previous == null)
            {
                return -1.0f;
            }
            var chargingEvent = evt as ChargingEvent;
            var deltaOdometer = chargingEvent.Odometer - previous.Odometer;

            if (settingsService.QuerySettings().FuelConsumption == Settings.FuelConsumptionType.LiterPer10km)
            {
                var consumption = (chargingEvent.Energy / deltaOdometer) * 10.0f;
                return consumption;
            }
            else
            {
                // MPG
                var consumption = deltaOdometer / chargingEvent.Energy;
                return consumption;
            }
        }

        private float CalculateFuelConsumption(Event evt, List<Event> events)
        {
            var fuelEvents = events.Where(x => x.Type == EventType.Fuel).ToList().OrderBy(y => y.Odometer).Reverse();
            var previous = fuelEvents.FirstOrDefault(x => x.Odometer < evt.Odometer);
            if(previous == null)
            {
                return -1.0f;
            }
            var fuelEvent = evt as FuelEvent;
            var deltaOdometer = fuelEvent.Odometer - previous.Odometer;

            if (settingsService.QuerySettings().FuelConsumption == Settings.FuelConsumptionType.LiterPer10km)
            {
                var consumption = (fuelEvent.Volume / deltaOdometer) * 10.0f;
                return consumption;
            }
            else
            {
                // MPG
                var consumption = deltaOdometer / fuelEvent.Volume;
                return consumption;
            }
        }

        private async Task HandleEventDeletedClicked(Guid eventId)
        {
            var evt = eventService.QueryEventById(eventId);
            var res = await dialogService.DisplayMessageQuestionAsync("Delete event of type [" + evt.Type + "]",
                                                          "You are about to delete the selected event, are you sure you want to proceed?");
            if (res == AnswerType.Yes)
            {
                await eventService.DeleteAsync(eventId);
            }
        }

        private Task HandleEditEventClicked(Guid eventId)
        {
            parent.Frame.Navigate(typeof(EditEventPage), eventId);
            return Task.CompletedTask;
        }

        private void HandleSettingsUpdated(Settings settings)
        {
            var selection = SelectedVehicleEvent?.Id;
            if(SelectedVehicle == null)
            {
                return;
            }

            vehicleEvents.Clear();
            var events = eventService.QueryAllEventsByVehicleId(SelectedVehicle.Id);
            foreach (var evt in events)
            {
                if (evt.Type == EventType.Fuel)
                {
                    var fEvt = new FuelEventExtended(evt as FuelEvent, CalculateFuelConsumption(evt, events));
                    vehicleEvents.Add(fEvt);
                }
                else if(evt.Type == EventType.Charging)
                {
                    var cEvt = new ChargingEventExtended(evt as ChargingEvent, CalculateEnergyConsumption(evt, events));
                    vehicleEvents.Add(cEvt);
                }
                else
                {
                    vehicleEvents.Add(evt);
                }
            }
            SelectedVehicleEvent = vehicleEvents.Where(x => x.Id == selection).FirstOrDefault();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddEventCommand { get; }

        public IAsyncCommand<Guid> EditEventCommand { get; }

        public IAsyncCommand<Guid> DeleteEventCommand { get; }

        public ObservableCollection<Vehicle> Vehicles { get; set; }

        public ICollectionView VehicleEvents => cvs;

        public Vehicle SelectedVehicle
        {
            get { return selectedVehicle; }
            set
            {
                if (selectedVehicle != value)
                {
                    selectedVehicle = value;
                    vehicleService.SelectedVehicleId = value == null ? Guid.Empty : value.Id;
                    (AddEventCommand as RelayCommand).RaiseCanExecuteChanged();
                    OnPropertyChanged("SelectedVehicle");
                }
            }
        }

        public Event SelectedVehicleEvent
        {
            get { return selectedVehicleEvent; }
            set
            {
                if (selectedVehicleEvent != value)
                {
                    selectedVehicleEvent = value;
                    OnPropertyChanged("SelectedVehicleEvent");
                }
            }
        }

        private void HandleAddEventClicked()
        {
            parent.Frame.Navigate(typeof(NewEventPage));
            eventService.PrepareNewEvent(SelectedVehicle.Id);
        }

        private bool CanAddEvent()
        {
            return SelectedVehicle != null;
        }

        private void HandleSelectedVehicleChanged(Guid vehicleId)
        {
            vehicleEvents.Clear();
            var events = eventService.QueryAllEventsByVehicleId(vehicleId);
            foreach (var evt in events)
            {
                if (evt.Type == EventType.Fuel)
                {
                    var fEvt = new FuelEventExtended(evt as FuelEvent, CalculateFuelConsumption(evt, events));
                    vehicleEvents.Add(fEvt);
                }
                else if(evt.Type == EventType.Charging)
                {
                    var cEvt = new ChargingEventExtended(evt as ChargingEvent, CalculateEnergyConsumption(evt, events));
                    vehicleEvents.Add(cEvt);
                }
                else
                {
                    vehicleEvents.Add(evt);
                }
            }
            SelectedVehicleEvent = vehicleEvents.Count() > 0 ? vehicleEvents.First() : null;
            SelectedVehicle = Vehicles.Where(x => x.Id == vehicleId).FirstOrDefault();
        }

        private void HandleEventDeleted(Guid eventId)
        {
            var item = vehicleEvents.FirstOrDefault(x => x.Id == eventId);
            if(item != null)
            {
                vehicleEvents.Remove(item);
            }
        }

        private void HandleEventCreated(Guid eventId)
        {
            var evt = eventService.QueryEventById(eventId);
            if (evt.VehicleId == SelectedVehicle.Id)
            {
                var events = eventService.QueryAllEventsByVehicleId(evt.VehicleId);
                if (evt.Type == EventType.Fuel)
                {
                    var fEvt = new FuelEventExtended(evt as FuelEvent, CalculateFuelConsumption(evt, events));
                    vehicleEvents.Add(fEvt);
                }
                else if(evt.Type == EventType.Charging)
                {
                    var cEvt = new ChargingEventExtended(evt as ChargingEvent, CalculateEnergyConsumption(evt, events));
                    vehicleEvents.Add(cEvt);
                }
                else
                {
                    vehicleEvents.Add(evt);
                }

                SelectedVehicleEvent = vehicleEvents.Where(x => x.Id == eventId).Single();
            }
        }

        private void HandleEventUpdated(Guid eventId)
        {
            var evt = eventService.QueryEventById(eventId);
            {

                var item = vehicleEvents.Where(x => x.Id == eventId).FirstOrDefault();
                if(item == null)
                {
                    return;
                }
                int i = vehicleEvents.IndexOf(item);

                var events = eventService.QueryAllEventsByVehicleId(evt.VehicleId);
                if (evt.Type == EventType.Fuel)
                {
                    var fEvt = new FuelEventExtended(evt as FuelEvent, CalculateFuelConsumption(evt, events));
                    vehicleEvents[i] = fEvt;
                }
                else if(evt.Type == EventType.Charging)
                {
                    var cEvt = new ChargingEventExtended(evt as ChargingEvent, CalculateEnergyConsumption(evt, events));
                    vehicleEvents[i] = cEvt;
                }
                else
                {
                    vehicleEvents[i] = evt;
                }
            }
        }

        private void HandleVehicleCreated(Guid vehicleId)
        {
            var id = SelectedVehicle != null ? SelectedVehicle.Id : Guid.Empty;
            Vehicles.Add(vehicleService.QueryVehicleById(vehicleId));

            if(id == Guid.Empty)
            {
                SelectedVehicle = Vehicles.First();
            }
            else
            {
                var selectedItem = Vehicles.Where(x => x.Id == id).FirstOrDefault();
                SelectedVehicle = selectedItem;
            }
        }

        private void HandleVehicleUpdated(Guid vehicleId)
        {
            var id = SelectedVehicle != null ? SelectedVehicle.Id : Guid.Empty;
            Vehicles.Clear();
            foreach (var vehicle in vehicleService.QueryAllVehicles())
            {
                Vehicles.Add(vehicle);
            }

            if (id == Guid.Empty)
            {
                SelectedVehicle = Vehicles.First();
            }
            else
            {
                var selectedItem = Vehicles.Where(x => x.Id == id).FirstOrDefault();
                SelectedVehicle = selectedItem;
            }
        }

        private void HandleVehicleDeleted(Guid vehicleId)
        {
            if (SelectedVehicle == null)
            {
                return;
            }
            var selectedId = SelectedVehicle.Id;
            var item = Vehicles.Where(x => x.Id == vehicleId).FirstOrDefault();
            Vehicles.Remove(item);
            if(vehicleId == selectedId)
            {
                SelectedVehicle = Vehicles.FirstOrDefault();
            }

            //Task.Run(async () =>
            //{
            //    await eventService.DeleteAllByVehicleIdAsync(vehicleId);
            //});
        }

        private void HandleAttachmentDeleted(Attachment attachment)
        {
            HandleEventUpdated(attachment.EventId);
        }

        private void HandleAttachmentAdded(Attachment attachment)
        {
            HandleEventUpdated(attachment.EventId);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
