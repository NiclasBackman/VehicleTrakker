using NavigationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VehicleTrakker.Services;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using static VehicleTrakker.DataDefinitions.Settings;

namespace VehicleTrakker.ViewModels
{
    public class SettingsPageViewModel : BindableBase
    {
        private readonly SettingsService settingsService;
        private readonly GeoService geoService;
        private bool isMetric;
        private string selectedCurrency;
        private int reminderMargin;
        private EventTypeSelectionVisualizationTypeImageMapping selectedEventSelectionVisualization;
        private bool exportImportIsEnabled;
        private MapProjectionImageMapping selectedMapProjection;
        private MapStyleImageMapping selectedMapStyle;
        private bool geoLocationIsEnabled;

        public SettingsPageViewModel() : base()
        {
            settingsService = SettingsService.Instance;
            geoService = GeoService.Instance;
            geoService.LocationPermissionUpdatedObservable.Subscribe(HandleLocationPermissionUpdated);
            GeoLocationIsEnabled = geoService.AccessStatus == GeolocationAccessStatus.Allowed;

            var settings = settingsService.QuerySettings();
            isMetric = settings.DistanceUnit == "km";
            AllCurrencies = settingsService.AllCultures.Select(x => x.NumberFormat.CurrencySymbol).Distinct().ToList();
            var symbol = settingsService.CurrencyCodeToCurrencySymbol(settings.CurrencyCultureCode);
            selectedCurrency = AllCurrencies.Where(x => x == symbol).FirstOrDefault();
            reminderMargin = settings.ReminderMargin;

            var list = Enum.GetValues(typeof(EventTypeSelectionVisualizationType));
            selectedEventSelectionVisualization = EventSelectionVisualizations.Where(x => x.Type == settings.EventSelectionVisualization).FirstOrDefault();

            exportImportIsEnabled = settings.ExportImportIsEnabled;

            selectedMapProjection = MapProjections.Where(x => x.Type == settings.SelectedMapProjection).FirstOrDefault();

            selectedMapStyle = MapStyles.Where(x => x.Type == settings.SelectedMapStyle).FirstOrDefault();
        }

        public ObservableCollection<MapProjectionImageMapping> MapProjections => MapProjectionImageMapping.AllMapProjections;

        public ObservableCollection<MapStyleImageMapping> MapStyles => MapStyleImageMapping.AllMapStyles;

        public MapProjectionImageMapping SelectedMapProjection
        {
            get { return selectedMapProjection; }
            set
            {
                selectedMapProjection = value;
                Task.Run(async () =>
                {
                    await settingsService.SetSelectedMapProjectionAsync(selectedMapProjection.Type);
                });
                OnPropertyChanged("SelectedMapProjection");
            }
        }

        public bool GeoLocationIsEnabled
        {
            get { return geoLocationIsEnabled; }
            set
            {
                geoLocationIsEnabled = value;
                OnPropertyChanged("GeoLocationIsEnabled");
            }
        }

        public MapStyleImageMapping SelectedMapStyle
        {
            get { return selectedMapStyle; }
            set
            {
                selectedMapStyle = value;
                Task.Run(async () =>
                {
                    await settingsService.SetSelectedMapStyleAsync(selectedMapStyle.Type);
                });
                OnPropertyChanged("SelectedMapStyle");
            }
        }

        public List<string> AllCurrencies { get; }

        public ObservableCollection<EventTypeSelectionVisualizationTypeImageMapping> EventSelectionVisualizations => EventTypeSelectionVisualizationTypeImageMapping.AllEventTypeVisualizations;

        public int ReminderMargin
        {
            get { return reminderMargin; }
            set
            {
                reminderMargin = value;
                Task.Run(async () =>
                {
                    await settingsService.SetReminderMarginAsync(reminderMargin);
                });
                OnPropertyChanged("ReminderMargin");
            }
        }

        public EventTypeSelectionVisualizationTypeImageMapping SelectedEventSelectionVisualization
        {
            get { return selectedEventSelectionVisualization; }
            set
            {
                selectedEventSelectionVisualization = value;
                Task.Run(async () =>
                {
                    await settingsService.SetSelectedEventSelectionVisualizationAsync(selectedEventSelectionVisualization.Type);
                });
                OnPropertyChanged("SelectedEventSelectionVisualization");
            }
        }

        public string SelectedCurrency
        {
            get { return selectedCurrency; }
            set
            {
                selectedCurrency = value;
                Task.Run(async () =>
                {
                    await settingsService.SetCurrencyAsync(selectedCurrency);
                });
                OnPropertyChanged("SelectedCurrency");
            }
        }

        public bool IsMetric
        {
            get { return isMetric; }
            set
            {
                if(isMetric != value)
                {
                    isMetric = value;
                    Task.Run(async () => 
                    {
                        await settingsService.SetMetricAsync(isMetric);
                    });
                    OnPropertyChanged("IsMetric");
                }
            }
        }

        public bool ExportImportIsEnabled
        {
            get { return exportImportIsEnabled; }
            set
            {
                exportImportIsEnabled = value;
                Task.Run(async () =>
                {
                    await settingsService.SetExportImportIsEnabled(exportImportIsEnabled);
                });
                OnPropertyChanged("ExportImportIsEnabled");
            }
        }

        private void HandleLocationPermissionUpdated(GeolocationAccessStatus access)
        {
            GeoLocationIsEnabled = access == GeolocationAccessStatus.Allowed;
        }
    }
}
