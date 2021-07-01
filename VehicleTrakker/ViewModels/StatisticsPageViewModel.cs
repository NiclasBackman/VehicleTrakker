using NavigationTest;
using NavigationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Services;
using static VehicleTrakker.DataDefinitions.Settings;

namespace VehicleTrakker.ViewModels
{
    public class StatisticsEntry
    {
        public DateTime TimeStamp { get; set; }

        public decimal Amount { get; set; }

        public EventType Category { get; set; }
    }

    public class StatisticsPageViewModel : BindableBase
    {
        private IEnumerable<StatisticsEntry> summaryStatistics;
        private Vehicle selectedVehicle;
        private readonly VehicleService vehicleService;
        private readonly EventService eventService;
        private readonly SettingsService settingsService;
        private string fuelConsumptionTextHeader;
        private string energyConsumptionTextHeader;
        private string currencyLabel;
        private ObservableCollection<IEnumerable<StatisticsEntry>> costTrend;
        private List<IEnumerable<StatisticsEntry>> fuelConsumptionSerie;
        private List<IEnumerable<StatisticsEntry>> energyConsumptionSerie;

        public StatisticsPageViewModel()
        {
            vehicleService = VehicleService.Instance;
            eventService = EventService.Instance;
            settingsService = SettingsService.Instance;

            Vehicles = new ObservableCollection<Vehicle>();
            foreach (var vehicle in vehicleService.QueryAllVehicles())
            {
                Vehicles.Add(vehicle);
            }
            SelectedVehicle = Vehicles.Where(x => x.Id == vehicleService.SelectedVehicleId).FirstOrDefault();
            vehicleService.SelectedVehicleChangedObservable.Subscribe(HandleSelectedVehicleChanged);
            vehicleService.VehicleDeletedObservable.Subscribe(HandleVehicleDeleted);
            vehicleService.VehicleCreatedObservable.Subscribe(HandleVehicleCreated);
            vehicleService.VehicleUpdatedObservable.Subscribe(HandleVehicleUpdated);
            eventService.EventCreatedObservable.Subscribe(HandleEventsModified);
            eventService.EventDeletedObservable.Subscribe(HandleEventsModified);
            eventService.EventUpdatedObservable.Subscribe(HandleEventsModified);
            settingsService.SettingsUpdatedObservable.Subscribe(HandleSettingsUpdated);
            var settings = settingsService.QuerySettings();
            FuelConsumptionTextHeader = $"Fuel consumption ({settings.FuelConsumption.ToDescriptionString()})";
            EnergyConsumptionTextHeader = GenerateEnergyConsumptionHeader(settings);
            CurrencyLabel = $"({SettingsService.Instance.CurrencyCodeToCurrencySymbol(settings.CurrencyCultureCode)})";
            CostTrend = new ObservableCollection<IEnumerable<StatisticsEntry>>();
            if(SelectedVehicle != null)
            {
                Populate(SelectedVehicle.Id);
            }
        }


        private string GenerateEnergyConsumptionHeader(Settings settings)
        {
            if (settings.FuelConsumption == FuelConsumptionType.LiterPer10km)
            {
                return "Energy consumption (" + settings.EnergyUnit + "/10" + settings.DistanceUnit + ")";
            }
            else
            {
                return "Energy consumption (" + settings.DistanceUnit + "/" + settings.EnergyUnit + ")";
            }
        }

        private void HandleSettingsUpdated(Settings settings)
        {
            FuelConsumptionTextHeader = $"Fuel consumption ({settings.FuelConsumption.ToDescriptionString()})";
            EnergyConsumptionTextHeader = GenerateEnergyConsumptionHeader(settings);
            CurrencyLabel = $"({SettingsService.Instance.CurrencyCodeToCurrencySymbol(settings.CurrencyCultureCode)})";
            if (SelectedVehicle != null)
            {
                Populate(SelectedVehicle.Id);
            }
        }

        private void HandleEventsModified(Guid eventId)
        {
            var evt = eventService.QueryEventById(eventId);
            if(evt != null)
            {
                if (evt.VehicleId == vehicleService.SelectedVehicleId)
                {
                    Populate(SelectedVehicle.Id);
                }
            }
        }

        private IEnumerable<StatisticsEntry> GenerateSamples(IEnumerable<Event> events, EventType eventType)
        {
            List<StatisticsEntry> samples = new List<StatisticsEntry>();

            //List<StatisticsEntry> samples2 = new List<StatisticsEntry>();
            // Grouping by month, update LabelFormat="{}{0:yy-MM}" on StatisticsPage
            //var data = events.Select(k => new { k.TimeStamp.Year, k.TimeStamp.Month, k.Cost }).GroupBy(x => new { x.Year, x.Month }, (key, group) => new
            //{
            //    yr = key.Year,
            //    mnth = key.Month,
            //    tCharge = group.Sum(k => k.Cost)
            //}).ToList();

            //var groupedEventList = events.GroupBy(x => x.TimeStamp.Month);
            //foreach(var evt in data)
            //{
            //    var month = new DateTime(evt.yr, evt.mnth, 1);

            //    var item = new StatisticsEntry()
            //    {
            //        TimeStamp = month,
            //        Amount = (decimal)evt.tCharge,
            //        Category = eventType
            //    };

            //    samples2.Add(item);
            //}

            foreach (var evt in events)
            {
                var item = new StatisticsEntry()
                {
                    TimeStamp = evt.TimeStamp.DateTime,
                    Amount = (decimal)evt.Cost,
                    Category = eventType
                };

                samples.Add(item);
            }
            return samples;
        }

        private void Populate(Guid vehicleId)
        {
            var stats = new List<StatisticsEntry>();
            var allEvents = eventService.QueryAllEventsByVehicleId(vehicleId);
            var types = allEvents.Select(x => x.Type).Distinct().ToList().Where(t => t != EventType.Action);

            // Generate summary
            foreach(var t in types)
            {
                var typesEvents = allEvents.Where(x => x.Type == t);
                var item = new StatisticsEntry()
                {
                    TimeStamp = DateTime.Parse("2021-04-14"),
                    Amount = (decimal)typesEvents.Sum(x => x.Cost),
                    Category = t
                };
                stats.Add(item);
            }
            SummaryStatistics = stats;

            // Generate fuel consumptions statistics
            {
                var fuelEvents = allEvents.Where(x => x.Type == DataDefinitions.EventType.Fuel).Cast<FuelEvent>();
                var noOf = fuelEvents?.Count();
                List<StatisticsEntry> fuelConsumptionSamples = new List<StatisticsEntry>();
                if (noOf > 0)
                {
                    FuelEvent current = fuelEvents.First() as FuelEvent;
                    for (int i = 1; i < noOf; i++)
                    {
                        var next = fuelEvents.ElementAt(i) as FuelEvent;
                        var deltaOdometer = current.Odometer - next.Odometer;
                        float consumption = 0.0f;// current.Volume / deltaOdometer;
                        if (settingsService.QuerySettings().FuelConsumption == Settings.FuelConsumptionType.LiterPer10km)
                        {
                            consumption = (current.Volume / deltaOdometer) * 10.0f;
                        }
                        else
                        {
                            // MPG
                            consumption = deltaOdometer / current.Volume;
                        }
                        var item = new StatisticsEntry()
                        {
                            TimeStamp = current.TimeStamp.DateTime,
                            Amount = Convert.ToDecimal(consumption),
                            Category = current.Type
                        };

                        fuelConsumptionSamples.Add(item);
                        current = next;
                    }

                    var f = fuelConsumptionSamples.Reverse<StatisticsEntry>().ToList();
                    FuelConsumptionSerie = new List<IEnumerable<StatisticsEntry>> { f };
                }
            }

            // Generate energy consumptions statistics
            {
                var energyEvents = allEvents.Where(x => x.Type == DataDefinitions.EventType.Charging).Cast<ChargingEvent>();
                var noOf = energyEvents?.Count();
                List<StatisticsEntry> energyConsumptionSamples = new List<StatisticsEntry>();
                if (noOf > 0)
                {
                    ChargingEvent current = energyEvents.First() as ChargingEvent;
                    for (int i = 1; i < noOf; i++)
                    {
                        var next = energyEvents.ElementAt(i) as ChargingEvent;
                        var deltaOdometer = current.Odometer - next.Odometer;
                        float consumption = 0.0f;// current.Volume / deltaOdometer;
                        if (settingsService.QuerySettings().FuelConsumption == Settings.FuelConsumptionType.LiterPer10km)
                        {
                            consumption = (current.Energy / deltaOdometer) * 10.0f;
                        }
                        else
                        {
                            // MPG
                            consumption = deltaOdometer / current.Energy;
                        }
                        var item = new StatisticsEntry()
                        {
                            TimeStamp = current.TimeStamp.DateTime,
                            Amount = Convert.ToDecimal(consumption),
                            Category = current.Type
                        };

                        energyConsumptionSamples.Add(item);
                        current = next;
                    }

                    var f = energyConsumptionSamples.Reverse<StatisticsEntry>().ToList();
                    EnergyConsumptionSerie = new List<IEnumerable<StatisticsEntry>> { f };
                }
            }

            // Generate cost trends
            CostTrend = new ObservableCollection<IEnumerable<StatisticsEntry>>();
            //var allTypes = allEvents.Select(o => o.Type).Distinct().ToList();
            //foreach (var eventType in allTypes)
            foreach (var eventType in types)
                {
                    var samples = GenerateSamples(allEvents.Where(x => x.Type == eventType).ToList(), eventType);
                CostTrend.Add(samples);
            }
        }

        
        public string EnergyConsumptionTextHeader
        {
            get
            {
                return energyConsumptionTextHeader;
            }
            set
            {
                energyConsumptionTextHeader = value;
                OnPropertyChanged("EnergyConsumptionTextHeader");
            }
        }

        public string FuelConsumptionTextHeader
        {
            get
            {
                return fuelConsumptionTextHeader;
            }
            set
            {
                fuelConsumptionTextHeader = value;
                OnPropertyChanged("FuelConsumptionTextHeader");
            }
        }

        public string CurrencyLabel
        {
            get
            {
                return currencyLabel;
            }
            set
            {
                currencyLabel = value;
                OnPropertyChanged("CurrencyLabel");
            }
        }

        public IEnumerable<StatisticsEntry> SummaryStatistics
        {
            get
            {
                return summaryStatistics;
            }
            set
            {
                summaryStatistics = value;
                OnPropertyChanged("SummaryStatistics");
            }
        }


        public List<IEnumerable<StatisticsEntry>> EnergyConsumptionSerie
        {
            get
            {
                return energyConsumptionSerie;
            }
            set
            {
                energyConsumptionSerie = value;
                OnPropertyChanged("EnergyConsumptionSerie");
            }
        }

        public List<IEnumerable<StatisticsEntry>> FuelConsumptionSerie
        {
            get
            {
                return fuelConsumptionSerie;
            }
            set
            {
                fuelConsumptionSerie = value;
                OnPropertyChanged("FuelConsumptionSerie");
            }
        }

        public ObservableCollection<IEnumerable<StatisticsEntry>> CostTrend
        {
            get
            {
                return costTrend;
            }
            set
            {
                costTrend = value;
                OnPropertyChanged("CostTrend");
            }
        }

        public ObservableCollection<Vehicle> Vehicles { get; set; }

        public Vehicle SelectedVehicle
        {
            get { return selectedVehicle; }
            set
            {
                if (selectedVehicle != value)
                {
                    selectedVehicle = value;
                    vehicleService.SelectedVehicleId = value == null ? Guid.Empty : value.Id;
                    OnPropertyChanged("SelectedVehicle");
                }
            }
        }

        private void HandleSelectedVehicleChanged(Guid vehicleId)
        {
            SelectedVehicle = Vehicles.Where(x => x.Id == vehicleId).FirstOrDefault();
            Populate(vehicleId);
        }

        private void HandleVehicleCreated(Guid vehicleId)
        {
            var id = SelectedVehicle != null ? SelectedVehicle.Id : Guid.Empty;
            Vehicles.Add(vehicleService.QueryVehicleById(vehicleId));

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

        //private void HandleVehicleSaved(Guid vehicleId)
        //{
        //    var id = SelectedVehicle != null ? SelectedVehicle.Id : Guid.Empty;
        //    Vehicles.Clear();
        //    foreach (var vehicle in vehicleService.QueryAllVehicles())
        //    {
        //        Vehicles.Add(vehicle);
        //    }

        //    if (id == Guid.Empty)
        //    {
        //        SelectedVehicle = Vehicles.First();
        //    }
        //    else
        //    {
        //        var selectedItem = Vehicles.Where(x => x.Id == id).FirstOrDefault();
        //        SelectedVehicle = selectedItem;
        //    }
        //}

        private void HandleVehicleDeleted(Guid vehicleId)
        {
            var selectedId = SelectedVehicle == null ? Guid.Empty : SelectedVehicle.Id;
            var item = Vehicles.Where(x => x.Id == vehicleId).FirstOrDefault();
            Vehicles.Remove(item);
            if (vehicleId == selectedId)
            {
                SelectedVehicle = Vehicles.FirstOrDefault();
            }
        }
    }
}
