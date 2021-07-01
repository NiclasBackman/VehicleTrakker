using NavigationTest.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace VehicleTrakker.ViewModels
{
    public class AboutPageViewModel : BindableBase
    {
        private Uri mailUrl;
        private string appVersion;
        private ObservableCollection<ThirdPartyProduct> thirdPartyProducts;

        public AboutPageViewModel()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            appVersion = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            var mailUrl = $"mailto:feedback.zalcinsoft@gmail.com?subject={AppName} ({AppVersion}) - Feedback&amp";
            MailUrl = new Uri(mailUrl);
            thirdPartyProducts = new ObservableCollection<ThirdPartyProduct>();
            var a = typeof(JsonConvert).GetTypeInfo().Assembly;
            thirdPartyProducts.Add(new ThirdPartyProduct("AsyncAwaitBestPractices.MVVM", "5.1.0", "https://github.com/brminnick/AsyncAwaitBestPractices"));
            thirdPartyProducts.Add(new ThirdPartyProduct("Microsoft.NETCore.UniversalWindowsPlatform", "6.2.12", "https://github.com/Microsoft/dotnet/blob/master/releases/UWP/README.md"));
            thirdPartyProducts.Add(new ThirdPartyProduct("Microsoft.Toolkit.Uwp.Notifications", "7.0.2", "https://github.com/windows-toolkit/WindowsCommunityToolkit"));
            thirdPartyProducts.Add(new ThirdPartyProduct("Microsoft.Toolkit.Uwp.Controls", "7.0.2", "https://github.com/windows-toolkit/WindowsCommunityToolkit"));
            thirdPartyProducts.Add(new ThirdPartyProduct("Microsoft.UI.Xaml", "2.5.0", "https://github.com/microsoft/microsoft-ui-xaml"));
            thirdPartyProducts.Add(new ThirdPartyProduct("Newtonsoft.Json", "13.0.1", "https://www.newtonsoft.com/json"));
            thirdPartyProducts.Add(new ThirdPartyProduct("Telerik.UI.for.UniversalWindowsPlatform", "1.0.2.5", "https://www.telerik.com/universal-windows-platform-ui"));
        }

        public string AppName => Package.Current.DisplayName;

        public string AppVersion => appVersion;

        public string AppPublisher => Package.Current.PublisherDisplayName;

        public Uri MailUrl
        {
            get
            {
                return mailUrl;
            }
            set
            {
                mailUrl = value;
                OnPropertyChanged("MailUrl");
            }
        }

       public ObservableCollection<ThirdPartyProduct> ThirdPartyProducts => thirdPartyProducts;
    }
}
