using NavigationTest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Maps;
using static VehicleTrakker.DataDefinitions.Settings;

namespace VehicleTrakker.Services
{
    public sealed class SettingsService
    {
        private static readonly SettingsService instance = new SettingsService();
        private readonly string path = Path.Combine("ZalcinSoft", "VehicleTrakker", "settings.json");
        private Settings settings;
        private const int DataVersion = 1;
        private StorageMetaData metadata = new StorageMetaData(DataVersion, "Settings");

        static SettingsService()
        {
        }

        private SettingsService()
        {
            settings = new Settings
            {
                DistanceUnit = "km",
                CurrencyCultureCode = "sv-SE",
                VolumeUnit = "liter",
                EnergyUnit = "kWh",
                FuelConsumption = FuelConsumptionType.LiterPer10km,
                EventSelectionVisualization = EventTypeSelectionVisualizationType.Organized,
                ReminderMargin = 7, // 7 days
                ExportImportIsEnabled = false
            };
            Load();
            SettingsUpdatedObservable = new ObservableProperty<Settings>();
        }

        public static SettingsService Instance
        {
            get
            {
                return instance;
            }
        }

        public ObservableProperty<Settings> SettingsUpdatedObservable
        {
            get;
        }

        public List<CultureInfo> AllCultures => CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList();

        public string CurrencyCodeToCurrencySymbol(string currencyCode)
        {
            var c = AllCultures.Where(x => x.Name == currencyCode).FirstOrDefault();
            return c.NumberFormat.CurrencySymbol;
        }

        public Settings QuerySettings()
        {
            return settings;
        }

        public async Task SetMetricAsync(bool isMetric)
        {
            if(isMetric)
            {
                settings.DistanceUnit = "km";
                settings.VolumeUnit = "liter";
                settings.FuelConsumption = FuelConsumptionType.LiterPer10km;
            }
            else
            {
                settings.DistanceUnit = "mi";
                settings.VolumeUnit = "gallon";
                settings.FuelConsumption = FuelConsumptionType.MilesPerGallon;
            }
            await UpdateAsync(settings);
        }

        public async Task SetCurrencyAsync(string currency)
        {
            var culture = AllCultures.Where(x => x.NumberFormat.CurrencySymbol == currency).FirstOrDefault();
            if(culture == null)
            {
                return;
            }

            settings.CurrencyCultureCode = culture.Name;
            await UpdateAsync(settings);
        }

        public async Task SetSelectedMapStyleAsync(MapStyle mapStyle)
        {
            settings.SelectedMapStyle = mapStyle;
            await UpdateAsync(settings);
        }

        public async Task SetSelectedMapProjectionAsync(MapProjection mapProjection)
        {
            settings.SelectedMapProjection = mapProjection;
            await UpdateAsync(settings);
        }

        public async Task SetSelectedEventSelectionVisualizationAsync(EventTypeSelectionVisualizationType selectedEventSelectionVisualization)
        {
            settings.EventSelectionVisualization = selectedEventSelectionVisualization;
            await UpdateAsync(settings);
        }

        public async Task SetReminderMarginAsync(int reminderMargin)
        {
            settings.ReminderMargin = reminderMargin;
            await UpdateAsync(settings);
        }

        public async Task SetExportImportIsEnabled(bool exportImportIsEnabled)
        {
            settings.ExportImportIsEnabled = exportImportIsEnabled;
            await UpdateAsync(settings);
        }


        public async Task UpdateAsync(Settings s)
        {

            settings = s;
            await PersistAsync();
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
             () =>
             {
                 SettingsUpdatedObservable.Publish(s);
             });
        }

        private void Load()
        {
            var container = new PersistentDataContainer<Settings>();
            Task.Run(async () =>
            {
                await container.LoadAsync(path);
            }).Wait();

            if (container.Data != null)
            {
                settings = container.Data;

                // Migrate data...
                switch (container.MetaData.Version)
                {
                    case 1:
                        break;
                    default:
                        break;
                }
            }
            if (settings.SelectedMapStyle == MapStyle.None)
            {
                settings.SelectedMapStyle = MapStyle.Road;
            }
        }

        private async Task PersistAsync()
        {
            var container = new PersistentDataContainer<Settings>(settings, metadata);
            await container.PersistAsync(path);
        }
    }
}
