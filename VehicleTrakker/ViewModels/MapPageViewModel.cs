using NavigationTest;
using NavigationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Services;
using Windows.Devices.Geolocation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;

namespace VehicleTrakker.ViewModels
{
    public class MapPageViewModel : MapConfigurationSettingsBase
    {
        private VehicleService vehicleService;
        private EventService eventService;
        private SettingsService settingsService;
        private Vehicle selectedVehicle;
        private MapControl map;
        private const int DefaultZoomLevel = 16;

        public MapPageViewModel(MapControl map)
        {
            this.map = map;
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
            vehicleService.VehicleCreatedObservable.Subscribe(HandleVehicleCreated);
            vehicleService.VehicleDeletedObservable.Subscribe(HandleVehicleDeleted);
            vehicleService.VehicleUpdatedObservable.Subscribe(HandleVehicleUpdated);
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

        private void CreateAndAddPinToMap(Event evt)
        {
            var mapIcon = new MapIcon();
            mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Images/Map/mappin.png"));
            mapIcon.ZIndex = 0;
            mapIcon.Title = evt.TimeStamp.ToString("d") + " " + evt.Type.ToDescriptionString() + "@" + evt.Odometer + " " + settingsService.QuerySettings().DistanceUnit;
            var basicGeoLoc = new BasicGeoposition { Latitude = evt.Location.Latitude, Longitude = evt.Location.Longitude };
            mapIcon.Location = new Geopoint(basicGeoLoc);
            map.MapElements.Add(mapIcon);
        }

        public async void PopulateAsync(Guid vehicleId)
        {
            // Clear map and populate with new ones
            map.MapElements.Clear();

            var events = eventService.QueryAllEventsByVehicleId(vehicleId).Where(e => e.Location != null);
            if(events.Count() == 0)
            {
                var geopos = new BasicGeoposition
                {
                    Latitude = GeoService.Instance.CurrentDeviceLocation.Coordinate.Point.Position.Latitude,
                    Longitude = GeoService.Instance.CurrentDeviceLocation.Coordinate.Point.Position.Longitude
                };
                map.Center = new Geopoint(geopos);
                map.ZoomLevel = DefaultZoomLevel;
                return;
            }

            foreach (var evt in events)
            {
                CreateAndAddPinToMap(evt);
            }

            var points = map.MapElements.Cast<MapIcon>().Select(loc => loc.Location.Position);
            if(points.Count() > 1)
            {
                await map.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(points), new Thickness(100.0), MapAnimationKind.None);
            }
            else
            {
                map.Center = new Geopoint(points.First());
                map.ZoomLevel = DefaultZoomLevel;
            }
        }

        private void HandleSelectedVehicleChanged(Guid vehicleId)
        {
            PopulateAsync(vehicleId);
            SelectedVehicle = Vehicles.Where(x => x.Id == vehicleId).FirstOrDefault();
        }

        private void HandleVehicleDeleted(Guid vehicleId)
        {
            if(SelectedVehicle == null)
            {
                return;
            }

            var selectedId = SelectedVehicle.Id;
            var item = Vehicles.Where(x => x.Id == vehicleId).FirstOrDefault();
            Vehicles.Remove(item);
            if (vehicleId == selectedId)
            {
                SelectedVehicle = Vehicles.FirstOrDefault();
            }
        }

        private void HandleVehicleCreated(Guid vehicleId)
        {
            Vehicles.Add(vehicleService.QueryVehicleById(vehicleId));
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
    }
}
