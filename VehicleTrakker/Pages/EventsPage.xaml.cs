using VehicleTrakker.DataDefinitions;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NavigationTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventsPage : Page
    {
        public EventsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            DataContext = new EventsPageViewModel(this);
        }

        private void HandleListViewIsDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var item = ((sender as ListView).SelectedItem) as Event;
            var viewModel = DataContext as EventsPageViewModel;
            viewModel.EditEventCommand.ExecuteAsync(item.Id);
        }
    }
}
