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
    public sealed partial class EditEventPage : Page
    {
        public EditEventPage()
        {
            this.InitializeComponent();
            DataContext = new EditEventPageViewModel(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var eventId = (Guid)e.Parameter;
            base.OnNavigatedTo(e);
            var viewModel = eventUserControl.DataContext as EditableEventUserControlViewModel;
            viewModel.Populate(eventId);
        }

        public bool CanSaveEvent(Guid id)
        {
            var viewModel = eventUserControl.DataContext as EditableEventUserControlViewModel;
            return viewModel.CanSaveEvent(id);
        }
    }
}
