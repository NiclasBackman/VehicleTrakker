using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UserControlTest;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VehicleTrakker.UserControls
{
    public sealed partial class DistanceAttributeUserControl : UserControl
    {
        public DistanceAttributeUserControl()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var extension = new UserControlExtensions();
            var vm = (DataContext as DistanceAttributeUserControlViewModel);
            extension.SetNumberBoxNumberFormatter(numberBox, 1, 0);        }
        }
    }
