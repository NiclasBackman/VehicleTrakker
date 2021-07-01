using NavigationTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VehicleTrakker.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VehicleTrakker.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page
    {
        public MapPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            DataContext = new MapPageViewModel(map);
            Loaded += HandleMapPageLoaded;
        }

        private void HandleMapPageLoaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MapPageViewModel;
            vm.PopulateAsync(VehicleService.Instance.SelectedVehicleId);
        }

        private void HandleMapRightTapped(MapControl sender, MapRightTappedEventArgs args)
        {
            //var pt = new Windows.Foundation.Point(args.Position.X, args.Position.Y);
            //MapFlyout.ShowAt(sender, new Windows.Foundation.Point(args.Position.X, args.Position.Y));
        }
    }
}
