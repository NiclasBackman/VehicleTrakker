using AsyncAwaitBestPractices.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VehicleTrakker;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Services;
using VehicleTrakker.ViewModels;
using static VehicleTrakker.DataDefinitions.Vehicle;
using static VehicleTrakker.DialogHelper;

namespace NavigationTest
{

    public class VehicleUserControlViewModel : INotifyPropertyChanged
    {
        private string vehicleName;
        private string vehicleRegistrationNumber;
        private DateTimeOffset vehicleDateOfPurchase;
        private VehiclePersistenceState vehicleState;
        private VehicleBrand selectedVehicleBrand;
        private Guid id;
        private readonly VehicleService vehicleService;
        private readonly SettingsService settingsService;
        private readonly EventService eventService;
        private readonly DialogHelper dialogService;
        private bool isFavorite;
        private readonly List<ServiceInterval> allServiceIntervals;
        private ServiceInterval selectedServiceInterval;
        private int serviceDistance;
        private string distanceUnit;
        private bool exportImportIsEnabled;
        private EngineTypeEntry selectedEngineType;

        public VehicleUserControlViewModel()
        {
            dialogService = new DialogHelper();
            vehicleService = VehicleService.Instance;
            settingsService = SettingsService.Instance;
            eventService = EventService.Instance;
            vehicleService.VehicleCreatedObservable.Subscribe(HandleVehicleSaved);
            vehicleService.VehicleUpdatedObservable.Subscribe(HandleVehicleSaved);
            settingsService.SettingsUpdatedObservable.Subscribe(HandleSettingsUpdated);
            SaveVehicleCommand = new AsyncCommand(HandleVehicleSavedClicked, CanSaveVehicle);
            UndoChangesCommand = new AsyncCommand<Guid>(HandleUndoChangesClicked, CanUndoVehicle);
            DeleteVehicleCommand = new AsyncCommand(HandleVehicleDeletedClicked, AlwaysTrue);
            SetAsFavoriteCommand = new AsyncCommand(HandleSetAsFavoriteClickedAsync, CanSetAsFavorite);
            ExportVehicleCommand = new AsyncCommand<Guid>(HandleExportVehicleClicked, CanExportVehicle);
            VehicleBrands = new ObservableCollection<VehicleBrand>();
            foreach (var vehicle in VehicleBrand.AllVehicleBrands)
            {
                VehicleBrands.Add(vehicle);
            }

            selectedVehicleBrand = VehicleBrands.FirstOrDefault();

            allServiceIntervals = new List<ServiceInterval>();
            foreach (var e in Enum.GetValues(typeof(ServiceIntervalType)).Cast<ServiceIntervalType>().ToList())
            {
                allServiceIntervals.Add(new ServiceInterval(e));
            }
            selectedServiceInterval = allServiceIntervals.Where(x => x.Type == ServiceIntervalType.Every24Month).FirstOrDefault();
            serviceDistance = 20000;
            var settings = settingsService.QuerySettings();
            distanceUnit = "(" + settings.DistanceUnit + ")";
            exportImportIsEnabled = settings.ExportImportIsEnabled;
            vehicleDateOfPurchase = DateTime.Now;

            EngineTypes = new ObservableCollection<EngineTypeEntry>
            {
                new EngineTypeEntry(EngineType.ICE),
                new EngineTypeEntry(EngineType.Hybrid),
                new EngineTypeEntry(EngineType.PureEv)
            };

            SelectedEngineType = EngineTypes.First();

            VehicleState = VehiclePersistenceState.Prepared;
        }

        public VehicleUserControlViewModel(Vehicle vehicle, VehiclePersistenceState state = VehiclePersistenceState.Saved) : this()
        {
            selectedVehicleBrand = VehicleBrands.Where(x => x.Name == vehicle.Brand).FirstOrDefault();
            vehicleDateOfPurchase = vehicle.DateOfPurchase;
            vehicleName = vehicle.Name;
            vehicleRegistrationNumber = vehicle.RegistrationNumber;
            vehicleState = state;
            Id = vehicle.Id;
            IsFavorite = vehicle.IsFavorite;
            selectedServiceInterval = allServiceIntervals.Where(x => x.Type == vehicle.ServiceInterval).FirstOrDefault();
            serviceDistance = vehicle.ServiceDistance;
            selectedEngineType = EngineTypes.Where(x => x.EngineType == vehicle.EngineType).FirstOrDefault();
        }

        public ObservableCollection<EngineTypeEntry> EngineTypes { get; }

        public EngineTypeEntry SelectedEngineType
        {
            get { return selectedEngineType; }
            set
            {
                selectedEngineType = value;
                OnPropertyChanged("SelectedEngineType");
                UpdateState();
                SaveVehicleCommand.RaiseCanExecuteChanged();
            }
        }

        public List<ServiceInterval> AllServiceIntervals => allServiceIntervals;

        public bool ExportImportIsEnabled
        {
            get { return exportImportIsEnabled; }
            set
            {
                if(exportImportIsEnabled != value)
                {
                    exportImportIsEnabled = value;
                    OnPropertyChanged("ExportImportIsEnabled");
                }
            }
        }

        public string DistanceUnit
        {
            get { return distanceUnit; }
            set
            {
                distanceUnit = value;
                OnPropertyChanged("DistanceUnit");
            }
        }

        public int ServiceDistance
        {
            get { return serviceDistance; }
            set
            {
                serviceDistance = value;
                OnPropertyChanged("ServiceDistance");
                UpdateState();
                SaveVehicleCommand.RaiseCanExecuteChanged();
            }
        }
        public ServiceInterval SelectedServiceInterval
        {
            get { return selectedServiceInterval; }
            set
            {
                selectedServiceInterval = value;
                OnPropertyChanged("SelectedServiceInterval");
                UpdateState();
                SaveVehicleCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTimeOffset Now
        {
            get { return DateTimeOffset.Now; }
        }

        public ObservableCollection<VehicleBrand> VehicleBrands { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public AsyncCommand SetAsFavoriteCommand { get; }

        public IAsyncCommand<Guid> ExportVehicleCommand { get; }

        public IAsyncCommand SaveVehicleCommand { get; }

        public IAsyncCommand<Guid> UndoChangesCommand { get; }

        public IAsyncCommand DeleteVehicleCommand { get; }
        
        private void UpdateState()
        {
            if(IsDirty())
            {
                VehicleState = VehiclePersistenceState.Edited;
                return;
            }

            VehicleState = VehiclePersistenceState.Saved;
        }

        public VehicleBrand SelectedVehicleBrand
        {
            get { return selectedVehicleBrand; }
            set
            {
                selectedVehicleBrand = value;
                UpdateState();
                OnPropertyChanged("SelectedVehicleBrand");
                SaveVehicleCommand.RaiseCanExecuteChanged();
            }
        }

        public VehiclePersistenceState VehicleState
        {
            get { return vehicleState; }
            set
            {
                if(vehicleState != value)
                {
                    vehicleState = value;
                    OnPropertyChanged("VehicleState");
                    SaveVehicleCommand.RaiseCanExecuteChanged();
                    ExportVehicleCommand.RaiseCanExecuteChanged();
                    UndoChangesCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string VehicleName
        {
            get { return vehicleName; }
            set
            {
                vehicleName = value;
                UpdateState();
                OnPropertyChanged("VehicleName");
                SaveVehicleCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsFavorite
        {
            get { return isFavorite; }
            set
            {
                isFavorite = value;
                SetAsFavoriteCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("IsFavorite");
            }
        }


        public string VehicleRegistrationNumber
        {
            get { return vehicleRegistrationNumber; }
            set
            {
                vehicleRegistrationNumber = value;
                UpdateState();
                OnPropertyChanged("VehicleRegistrationNumber");
            }
        }

        public DateTimeOffset VehicleDateOfPurchase
        {
            get { return vehicleDateOfPurchase; }
            set
            {
                vehicleDateOfPurchase = value;
                UpdateState();
                OnPropertyChanged("VehicleDateOfPurchase");
            }
        }

        public Guid Id { get => id; set => id = value; }

        private bool AlwaysTrue(object arg)
        {
            return true;
        }

        private bool CanSaveVehicle(object arg)
        {
            return !string.IsNullOrEmpty(VehicleName) && SelectedVehicleBrand != null &&
                ServiceDistance > 0 && IsDirty();
        }

        private bool IsDirty()
        {
            var persisted = vehicleService.QueryVehicleById(id);
            if(persisted != null)
            {
                if (persisted.Id == this.id &&
                    persisted.Brand == this.SelectedVehicleBrand.Name &&
                    persisted.DateOfPurchase.Date == this.VehicleDateOfPurchase.Date &&
                    persisted.Name == this.VehicleName &&
                    persisted.RegistrationNumber == this.VehicleRegistrationNumber &&
                    persisted.ServiceDistance == this.ServiceDistance &&
                    persisted.ServiceInterval == this.SelectedServiceInterval.Type &&
                    persisted.EngineType == this.SelectedEngineType.EngineType)
                {
                    VehicleState = VehiclePersistenceState.Saved;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private bool CanUndoVehicle(object arg)
        {
            return IsDirty();
        }

        private async Task HandleUndoChangesClicked(Guid vehicleId)
        {
            var res = await dialogService.DisplayMessageQuestionAsync("Undo changes for vehicle [" + VehicleName + "]",
                                              "You are about to undo the changes, are you sure you want to proceed?");
            if (res == AnswerType.Yes)
            {
                var vehicle = vehicleService.QueryVehicleById(vehicleId);
                SelectedVehicleBrand = VehicleBrands.Where(x => x.Name == vehicle.Brand).FirstOrDefault();
                VehicleDateOfPurchase = vehicle.DateOfPurchase;
                VehicleName = vehicle.Name;
                VehicleRegistrationNumber = vehicle.RegistrationNumber;
                VehicleState = VehiclePersistenceState.Saved;
                IsFavorite = vehicle.IsFavorite;
                SelectedServiceInterval = allServiceIntervals.Where(x => x.Type == vehicle.ServiceInterval).FirstOrDefault();
                ServiceDistance = vehicle.ServiceDistance;
            }
        }

        private async Task HandleVehicleSavedClicked()
        {
            var vehicle = vehicleService.QueryVehicleById(this.Id);
            await vehicleService.SaveAsync(new Vehicle(VehicleName, VehicleRegistrationNumber, VehicleDateOfPurchase.DateTime, SelectedVehicleBrand.Name,
                this.Id, this.IsFavorite, this.SelectedServiceInterval.Type, this.ServiceDistance, this.SelectedEngineType.EngineType));

            if(vehicle == null) // New one
            {
                await eventService.SaveAsync(new ActionEvent(ActionType.Acquired)
                {
                    Id = Guid.NewGuid(),
                    VehicleId = this.Id,
                    Odometer = 1,
                    TimeStamp = VehicleDateOfPurchase.DateTime
                });
            }
            SaveVehicleCommand.RaiseCanExecuteChanged();
        }

        private async Task HandleVehicleDeletedClicked()
        {
            var res = await dialogService.DisplayMessageQuestionAsync("Delete vehicle [" + VehicleName + "]",
                                              "You are about to delete the selected vehicle, are you sure you want to proceed?");
            if (res == AnswerType.Yes)
            {
                await vehicleService.DeleteAsync(Id);
            }
        }

        private bool CanSetAsFavorite(object arg)
        {
            var vehicle = vehicleService.QueryVehicleById(Id);
            return vehicle != null;
        }

        private async Task HandleSetAsFavoriteClickedAsync()
        {
            if(IsFavorite == true)
            {
                return;
            }

            await vehicleService.SetNewFavoriteAsync(Id);
        }

        private bool CanExportVehicle(object arg)
        {
            var res = IsDirty();
            return !res;
        }

        private async Task HandleExportVehicleClicked(Guid vehicleId)
        {
            var vehicle = vehicleService.QueryVehicleById(vehicleId);
            var savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Data export", new List<string>() { ".xml" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "Vehicle_" + vehicle.Name;
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file

                var res = await ApplicationService.Instance.ExportAsync(vehicleId, file);
                if(!string.IsNullOrEmpty(res))
                {
                    await dialogService.DisplayMessageAsync($"Export data for vehicle '{vehicle.Name}' to file", $"Failed to export data to {file.Path}, error: {res}");
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
                    await dialogService.DisplayMessageAsync($"Export data for vehicle '{vehicle.Name}' to file", $"Data successfully exported to {file.Path}");
                }
                else
                {
                    await dialogService.DisplayMessageAsync($"Export data for vehicle '{vehicle.Name}' to file", $"Failed to export data to {file.Path}");
                }
            }
            else
            {
                return;
            }
        }

        private void HandleVehicleSaved(Guid id)
        {
            if (id == this.Id)
            {
                VehicleState = VehiclePersistenceState.Saved;
                var vehicle = vehicleService.QueryVehicleById(id);
                IsFavorite = vehicle.IsFavorite;
            }
        }

        private void HandleSettingsUpdated(Settings settings)
        {
            DistanceUnit = "(" + settings.DistanceUnit + ")";
            ExportImportIsEnabled = settings.ExportImportIsEnabled;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
