using System;
using System.ComponentModel;
using System.Globalization;
using Windows.UI.Xaml.Controls.Maps;

namespace VehicleTrakker.DataDefinitions
{
    public class Settings
    {
        public enum FuelConsumptionType
        {
            [Description("l/10km")]
            LiterPer10km,
            [Description("mpg")]
            MilesPerGallon
        };

        public enum EventTypeSelectionVisualizationType
        {
            [Description("Organized layout")]
            Organized,
            [Description("Drop-down list")]
            DropDown
        };

        public Settings()
        {
            CurrencyCultureCode = CultureInfo.CurrentCulture.Name;
        }

        public string CurrencyCultureCode { get; set; }

        public string DistanceUnit { get; set; }

        public string VolumeUnit { get; set; }

        public string EnergyUnit { get; set; }

        public FuelConsumptionType FuelConsumption { get; set; }

        public int ReminderMargin { get; set; }

        public EventTypeSelectionVisualizationType EventSelectionVisualization { get; set; }

        public bool ExportImportIsEnabled { get; set; }

        public MapProjection SelectedMapProjection { get; set; }

        public MapStyle SelectedMapStyle { get; set; }
    }
}
