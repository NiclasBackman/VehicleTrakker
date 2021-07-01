using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using VehicleTrakker;
using VehicleTrakker.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static VehicleTrakker.DataDefinitions.Reminder;

namespace NavigationTest
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private Thread checkerThread;
        private ReminderService reminderService;
        private ApplicationService applicationService;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            reminderService = ReminderService.Instance;
            applicationService = ApplicationService.Instance;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
            checkerThread = new Thread(ReminderChecker.DoWork);
            checkerThread.IsBackground = true;
            checkerThread.Start();
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.Title = "Your garage's best friend";

            GeolocationAccessStatus accessStatus = await Geolocator.RequestAccessAsync();
            if(accessStatus != GeolocationAccessStatus.Allowed)
            {
                var dlg = new DialogHelper();
                await dlg.DisplayMessageAsync("Access denied to Location service", "In order for app to work correctly, please allow positioning, Settings -> Windows Security -> Find my device -> VehicleTrakker -> Enable");
            }
            GeoService.Instance.Initialize();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
        protected override void OnActivated(IActivatedEventArgs e)
        {
            // Handle notification activation
            if (e is ToastNotificationActivatedEventArgs toastActivationArgs)
            {
                // Obtain the arguments from the notification
                ToastArguments args = ToastArguments.Parse(toastActivationArgs.Argument);

                string actionValue;
                var action = args.TryGetValue("action", out actionValue);
                if (action && actionValue == ReminderToastConstants.ReminderIsExpiredAction)
                {
                    var reminderId = Guid.Parse(args[ReminderToastConstants.ReminderId]);
                    var reminder = reminderService.QueryReminderById(reminderId);
                    if (args.Contains(ReminderToastConstants.ReminderButtonAction))
                    {
                        // Obtain any user input (text boxes, menu selections) from the notification
                        var buttonAction = args[ReminderToastConstants.ReminderButtonAction];
                        if (buttonAction == ReminderToastConstants.ReminderButtonActionConfirmed)
                        {
                            reminder.State = ReminderState.Confirmed;
                            Task.Run(async () =>
                            {
                                await ReminderService.Instance.UpdateAsync(reminder);
                            });
                        }
                        else if (buttonAction == ReminderToastConstants.ReminderButtonActionPostpond)
                        {
                            ValueSet userInput = toastActivationArgs.UserInput;

                            int noOfDays = ReminderToastConstants.ItemTagToNumberOfDays(userInput[ReminderToastConstants.PostpondValueSelection] as string);
                            reminder.State = ReminderState.Idle;
                            reminder.ExpirationDate = reminder.ExpirationDate.AddDays(noOfDays);
                            Task.Run(async () =>
                            {
                                await ReminderService.Instance.UpdateAsync(reminder);
                            });
                        }
                    }
                }


                // TODO: Show the corresponding content
            }
        }
    }
}
