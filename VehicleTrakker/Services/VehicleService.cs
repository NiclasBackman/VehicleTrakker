using NavigationTest.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Services;

namespace NavigationTest
{
    public sealed class VehicleService
    {
        private static readonly VehicleService instance = new VehicleService();
        private List<Vehicle> allvehicles;
        private readonly string path = Path.Combine("ZalcinSoft", "VehicleTrakker", "vehicles.json");
        private Guid selectedVehicleId = Guid.Empty;
        private const int DataVersion = 1;
        private StorageMetaData metadata = new StorageMetaData(DataVersion, "Vehicles");
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static VehicleService()
        {
        }

        private VehicleService()
        {
            allvehicles = new List<Vehicle>();
            Load();
            VehicleCreatedObservable = new ObservableProperty<Guid>();
            VehicleUpdatedObservable = new ObservableProperty<Guid>();
            VehicleDeletedObservable = new ObservableProperty<Guid>();
            SelectedVehicleChangedObservable = new ObservableProperty<Guid>();
            SelectedVehicleId = allvehicles.Count > 0 ? allvehicles.FirstOrDefault(x => x.IsFavorite == true).Id : Guid.Empty;
        }

        public static VehicleService Instance
        {
            get
            {
                return instance;
            }
        }

        public ObservableProperty<Guid> SelectedVehicleChangedObservable
        {
            get;
        }

        public ObservableProperty<Guid> VehicleCreatedObservable
        {
            get;
        }

        public ObservableProperty<Guid> VehicleUpdatedObservable
        {
            get;
        }

        public ObservableProperty<Guid> VehicleDeletedObservable
        {
            get;
        }

        public Guid SelectedVehicleId 
        {
            get => selectedVehicleId;
            set
            {
                if(selectedVehicleId != value)
                {
                    selectedVehicleId = value;
                    SelectedVehicleChangedObservable.Publish(value);
                }
            }
        }

        public Vehicle PrepareNewVehicle()
        {
            var vehicle = new Vehicle
            {
                Name = string.Empty,
                RegistrationNumber = string.Empty,
                DateOfPurchase = DateTime.Now,
                Brand = VehicleBrand.AllVehicleBrands.FirstOrDefault().Name,
                Id = Guid.NewGuid(),
                IsFavorite = false,
                ServiceInterval = Vehicle.ServiceIntervalType.Every18Month,
                ServiceDistance = 30000,
                EngineType = EngineType.ICE
            };
            return vehicle;
        }

        public List<Vehicle> QueryAllVehicles()
        {
            if (allvehicles != null)
            {
                return allvehicles.Reverse<Vehicle>().ToList();
            }
            return new List<Vehicle>();
        }

        public Vehicle QueryVehicleById(Guid id)
        {
            return allvehicles.FirstOrDefault(x => x.Id == id);
        }

        public async Task SaveAsync(Vehicle vehicle)
        {
            OperationType op = OperationType.None;
            var current = allvehicles.Where(x => x.Id == vehicle.Id).FirstOrDefault();
            var idx = allvehicles.IndexOf(current);

            if(idx == -1)
            {
                if(allvehicles.Count == 0)
                {
                    vehicle.IsFavorite = true;
                }
                allvehicles.Add(vehicle);
                op = OperationType.Created;
            }
            else
            {
                allvehicles[idx] = vehicle;
                op = OperationType.Updated;
            }

            await PersistAsync();
            switch(op)
            {
                case OperationType.Created:
                    VehicleCreatedObservable.Publish(vehicle.Id);
                    break;
                case OperationType.Updated:
                    VehicleUpdatedObservable.Publish(vehicle.Id);
                    break;
                default:
                    throw new ArgumentException("Invalid operation type");
            }
        }

        public async Task SetNewFavoriteAsync(Guid vehicleId)
        {
            var vehicle = QueryVehicleById(vehicleId);
            if(vehicle == null)
            {
                return;
            }
            var currentFavorite = allvehicles.Where(x => x.IsFavorite).FirstOrDefault();
            vehicle.IsFavorite = true;
            currentFavorite.IsFavorite = false;
            await SaveAsync(vehicle);
            await SaveAsync(currentFavorite);
        }

        public async Task DeleteAsync(Guid vehicleId)
        {
            if(vehicleId == Guid.Empty)
            {
                // We have a non persistent candidate
                VehicleDeletedObservable.Publish(vehicleId);
                return;
            }
            var vehicle = QueryVehicleById(vehicleId);

            allvehicles.Remove(allvehicles.Where(x => x.Id == vehicleId).FirstOrDefault());
            await PersistAsync();
            VehicleDeletedObservable.Publish(vehicleId);
            if (allvehicles.Count > 0 && vehicle != null && vehicle.IsFavorite)
            {
                var candidate = allvehicles.FirstOrDefault();
                candidate.IsFavorite = true;
                await SaveAsync(candidate);
            }
        }

        private void Load()
        {
            var container = new PersistentDataContainer<List<Vehicle>>();
            Task.Run(async () =>
            {
                await container.LoadAsync(path);
            }).Wait();

            if (container.Data != null)
            {
                allvehicles = container.Data;

                // Migrate data...
                switch (container.MetaData.Version)
                {
                    case 1:
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task PersistAsync()
        {
            var container = new PersistentDataContainer<List<Vehicle>>(allvehicles, metadata);
            await container.PersistAsync(path);
        }
    }
}
