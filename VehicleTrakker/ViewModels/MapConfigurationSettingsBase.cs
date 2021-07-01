using NavigationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Services;
using Windows.UI.Xaml.Controls.Maps;

namespace VehicleTrakker.ViewModels
{
    public abstract class MapConfigurationSettingsBase : BindableBase
    {
        private MapStyle mapStyle;
        private MapProjection mapProjection;
        private SettingsService settingsService;

        public MapConfigurationSettingsBase()
        {
            settingsService = SettingsService.Instance;
            settingsService.SettingsUpdatedObservable.Subscribe(HandleSettingsUpdated);
            var settings = settingsService.QuerySettings();
            MapProjection = settings.SelectedMapProjection;
            MapStyle = settings.SelectedMapStyle;
        }

        private void HandleSettingsUpdated(Settings settings)
        {
            MapStyle = settings.SelectedMapStyle;
            MapProjection = settings.SelectedMapProjection;
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

    }
}
