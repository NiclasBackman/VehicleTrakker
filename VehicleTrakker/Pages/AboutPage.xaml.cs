using System;
using VehicleTrakker.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VehicleTrakker.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            DataContext = new AboutPageViewModel();
            //var mailUrl = $"mailto:feedback.zalcinsoft@gmail.com?subject={"package.DisplayName"} ({"versionText.Text"}) - Feedback&amp";
            //mailHyperLink.NavigateUri = new Uri(mailUrl);
        }
    }
}
