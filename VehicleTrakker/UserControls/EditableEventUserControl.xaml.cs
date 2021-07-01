using System;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Services;
using VehicleTrakker.ViewModels;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VehicleTrakker.UserControls
{
    public sealed partial class EditableEventUserControl : UserControl
    {
        private RandomAccessStreamReference mapIconStreamReference;
        private MapIcon mapIcon;
        private SettingsService settingsService;
        private Geopoint currentRmBLocation;

        public EditableEventUserControl()
        {
            var extension = new UserControlExtensions();
            this.InitializeComponent();
            this.Loaded += HandleUserControlLoaded;
            DataContext = new EditableEventUserControlViewModel(this);
            extension.SetNumberBoxNumberFormatter(costNumberBox, 0.01);
            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Images/Map/MapPin.png"));
            map.MapDoubleTapped += OnMapDoubleTapped;
            settingsService = SettingsService.Instance;
        }

        private void HandleUserControlLoaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as EditableEventUserControlViewModel;
            if (vm.CurrentEvent.Location != null)
            {
                var geopos = new BasicGeoposition
                {
                    Latitude = vm.CurrentEvent.Location.Latitude,
                    Longitude = vm.CurrentEvent.Location.Longitude
                };
                map.Center = new Geopoint(geopos);
            }
            else
            {
                var position = GeoService.Instance.CurrentDeviceLocation;
                map.Center = position?.Coordinate.Point;
            }
        }

        private async void UpdateMapAndViewModelAsync(Geopoint location)
        {
            if (mapIcon == null)
            {
                mapIcon = new MapIcon();
                mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);
                mapIcon.Image = mapIconStreamReference;
                mapIcon.ZIndex = 0;
                map.MapElements.Add(mapIcon);
            }
            var mapLocation = await MapLocationFinder.FindLocationsAtAsync(location);
            var vm = DataContext as EditableEventUserControlViewModel;
            if (mapLocation.Status == MapLocationFinderStatus.Success)
            {
                string address = mapLocation.Locations[0].Address.StreetNumber + " " + mapLocation.Locations[0].Address.Street;
                mapIcon.Title = address;
            }
            else
            {
                mapIcon.Title = vm.CurrentEvent.Type.ToDescriptionString() + "@" + vm.CurrentEvent.Odometer + " " + settingsService.QuerySettings().DistanceUnit;
            }
            mapIcon.Location = location;
            vm.Location = new EventGeoPosition(location.Position.Latitude, location.Position.Longitude);
        }

        private void HandleMapDoubleTapped(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {
            UpdateMapAndViewModelAsync(args.Location);
        }

        private async void OnMapDoubleTapped(MapControl sender, MapInputEventArgs args)
        {
            if(mapIcon != null)
            {
                var currentCamera = sender.ActualCamera;
                await sender.TrySetSceneAsync(MapScene.CreateFromCamera(currentCamera));
            }
        }

        private void HandleMapRightTapped(MapControl sender, MapRightTappedEventArgs args)
        {
            var pt = new Windows.Foundation.Point(args.Position.X, args.Position.Y);
            currentRmBLocation = args.Location;
            MapFlyout.ShowAt(sender, new Windows.Foundation.Point(args.Position.X, args.Position.Y));
        }

        public async void SetLocation(Geopoint location)
        {
            if (location != null)
            {
                mapIcon = new MapIcon();
                mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);
                mapIcon.Image = mapIconStreamReference;
                mapIcon.ZIndex = 0;
                map.MapElements.Add(mapIcon);
                var mapLocation = await MapLocationFinder.FindLocationsAtAsync(location);
                var vm = DataContext as EditableEventUserControlViewModel;
                if (mapLocation.Status == MapLocationFinderStatus.Success)
                {
                    string address = mapLocation.Locations[0].Address.StreetNumber + " " + mapLocation.Locations[0].Address.Street;
                    mapIcon.Title = address;
                }
                else
                {
                    mapIcon.Title = vm.CurrentEvent.Type.ToDescriptionString() + "@" + vm.CurrentEvent.Odometer + " " + settingsService.QuerySettings().DistanceUnit;
                }
                mapIcon.Location = location;
            }
            else
            {
                mapIcon = null;
                map.MapElements.Clear();
            }
        }

        private void HandleNewPositionHereClicked(object sender, RoutedEventArgs e)
        {
            UpdateMapAndViewModelAsync(currentRmBLocation);
        }
    }
}
