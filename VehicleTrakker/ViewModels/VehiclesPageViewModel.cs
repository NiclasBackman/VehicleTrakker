using AsyncAwaitBestPractices.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VehicleTrakker;
using VehicleTrakker.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace NavigationTest
{
    public enum EventState
    {
        Idle,
        Preparing
    }

    public class VehiclesPageViewModel : INotifyPropertyChanged
    {
        private readonly DialogHelper dialogService;
        private readonly VehicleService vehicleService;
        private readonly SettingsService settingsService;
        private bool exportImportIsEnabled;

        public VehiclesPageViewModel()
        {
            dialogService = new DialogHelper();
            vehicleService = VehicleService.Instance;
            settingsService = SettingsService.Instance;
            settingsService.SettingsUpdatedObservable.Subscribe(HandleSettingsUpdated);
            exportImportIsEnabled = settingsService.QuerySettings().ExportImportIsEnabled;
            Vehicles = new ObservableCollection<VehicleUserControl>();
            AddVehicleCommand = new AsyncCommand(HandleVehicleAddedClicked);
            ImportVehicleCommand = new AsyncCommand(HandleImportVehicleClicked);
            vehicleService.VehicleDeletedObservable.Subscribe(HandleVehicleDeleted);
            vehicleService.VehicleCreatedObservable.Subscribe(HandleVehicleCreated);
            foreach(var vehicle in vehicleService.QueryAllVehicles())
            {
                Vehicles.Add(new VehicleUserControl(vehicle));
            }
        }

        private void HandleSettingsUpdated(VehicleTrakker.DataDefinitions.Settings settings)
        {
            ExportImportIsEnabled = settings.ExportImportIsEnabled;
        }

        public bool ExportImportIsEnabled
        {
            get { return exportImportIsEnabled; }
            set
            {
                if (exportImportIsEnabled != value)
                {
                    exportImportIsEnabled = value;
                    OnPropertyChanged("ExportImportIsEnabled");
                }
            }
        }

        private void HandleVehicleDeleted(Guid id)
        {
            Vehicles.Remove(Vehicles.Where(x => (x.DataContext as VehicleUserControlViewModel).Id == id).FirstOrDefault());
        }

        private void HandleVehicleCreated(Guid vehicleId)
        {
            var viewModel = Vehicles.Where(x => (x.DataContext as VehicleUserControlViewModel).Id == vehicleId).FirstOrDefault();
            if(viewModel == null)
            {
                var vehicle = vehicleService.QueryVehicleById(vehicleId);
                Vehicles.Add(new VehicleUserControl(vehicle));
            }
        }

        public ObservableCollection<VehicleUserControl> Vehicles { get; }

        public IAsyncCommand AddVehicleCommand { get; }

        public IAsyncCommand ImportVehicleCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task HandleImportVehicleClicked()
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            openPicker.FileTypeFilter.Add(".xml");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file

                try
                {
                    var res = await ApplicationService.Instance.ImportAsync(file);
                }
                catch (Exception ex)
                {
                    await dialogService.DisplayMessageAsync("Import data from file", $"Failed to import file '{file.Path}',{Environment.NewLine}error: {ex.Message}");
                    return;
                }


                //await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);

                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    await dialogService.DisplayMessageAsync("Import data from file", $"File '{file.Path}' successfully imported.");
                }
                else
                {
                    await dialogService.DisplayMessageAsync("Import data from file", $"Failed to import file '{file.Path}'");
                }
            }
            else
            {
                return;
            }
        }

        private Task HandleVehicleAddedClicked()
        {
            var candidate = vehicleService.PrepareNewVehicle();
            Vehicles.Insert(0, new VehicleUserControl(candidate));
            return Task.CompletedTask;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
