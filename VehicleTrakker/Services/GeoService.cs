using NavigationTest;
using System;
using Windows.Devices.Geolocation;
using Windows.UI.Core;

namespace VehicleTrakker.Services
{
    public sealed class GeoService
    {
        private static readonly GeoService instance = new GeoService();
        private Geoposition currentDeviceLocation;
        private Geolocator geolocator;
        GeolocationAccessStatus accessStatus = GeolocationAccessStatus.Unspecified;

        static GeoService()
        {
        }

        private GeoService()
        {
            LocationPermissionUpdatedObservable = new ObservableProperty<GeolocationAccessStatus>();
            geolocator = new Geolocator()
            {
                DesiredAccuracy = PositionAccuracy.High
            };
            geolocator.AllowFallbackToConsentlessPositions();
            geolocator.StatusChanged += HandleGeoLocatorStatusChanged;
        }

        public ObservableProperty<GeolocationAccessStatus> LocationPermissionUpdatedObservable
        {
            get;
        }

        private void HandleGeoLocatorStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            Initialize();
        }

        public async void Initialize()
        {
            try
            {
                var currentAccessStatus = await Geolocator.RequestAccessAsync();
                currentDeviceLocation = await geolocator.GetGeopositionAsync();
                if (currentAccessStatus != accessStatus)
                {
                    accessStatus = currentAccessStatus;

                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                     () =>
                     {
                         LocationPermissionUpdatedObservable.Publish(accessStatus);
                     });
                }
            }
            catch(Exception)
            {
                // Do nothing...
            }
        }

        public Geoposition CurrentDeviceLocation => currentDeviceLocation;

        public static GeoService Instance
        {
            get
            {
                return instance;
            }
        }

        public GeolocationAccessStatus AccessStatus { get => accessStatus; }
    }
}
