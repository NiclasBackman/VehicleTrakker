using NavigationTest.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VehicleTrakker;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Services;
using Windows.Storage;

namespace NavigationTest
{
    public sealed partial class EventService
    {

        private static readonly EventService instance = new EventService();
        private List<Event> allEvents;
        private readonly string applicationAttachmentsPath = Path.Combine("ZalcinSoft", "VehicleTrakker", "Attachments");
        private readonly string path = Path.Combine("ZalcinSoft", "VehicleTrakker", "events.json");
        private readonly object eventLock = new object();
        private const int DataVersion = 1;
        private StorageMetaData metadata = new StorageMetaData(DataVersion, "Events");

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static EventService()
        {
        }

        private EventService()
        {
            allEvents = new List<Event>();
            Load();
            EventCreatedObservable = new ObservableProperty<Guid>();
            EventUpdatedObservable = new ObservableProperty<Guid>();
            EventDeletedObservable = new ObservableProperty<Guid>();
            EventPreparedObservable = new ObservableProperty<Event>();
            EventPreparedCancelledObservable = new ObservableProperty<Guid>();
            AttachmentAddedObservable = new ObservableProperty<Attachment>();
            AttachmentDeletedObservable = new ObservableProperty<Attachment>();
        }

        public static EventService Instance
        {
            get
            {
                return instance;
            }
        }

        public ObservableProperty<Guid> EventPreparedCancelledObservable
        {
            get;
        }

        public ObservableProperty<Event> EventPreparedObservable
        {
            get;
        }

        public ObservableProperty<Attachment> AttachmentDeletedObservable
        {
            get;
        }

        public ObservableProperty<Attachment> AttachmentAddedObservable
        {
            get;
        }

        public ObservablePropertyAsync<Guid> SelectedEventChangedObservableAsync
        {
            get;
        }

        public ObservableProperty<Guid> EventCreatedObservable
        {
            get;
        }

        public ObservableProperty<Guid> EventUpdatedObservable
        {
            get;
        }

        public ObservableProperty<Guid> EventDeletedObservable
        {
            get;
        }

        public List<Event> AllVehicles { get => allEvents; }

        public int GetMaxEventAttributeForVehicleId(Guid vehicleId, Func<Event, int> filter)
        {
            var events = allEvents.Where(x => x.VehicleId == vehicleId);
            if(events.Count() == 0)
            {
                return 0;
            }
            return allEvents.Where(x => x.VehicleId == vehicleId).Max(filter);
        }

        public Event QueryEventById(Guid id)
        {
            return allEvents.FirstOrDefault(x => x.Id == id);
        }

        public void PrepareNewEvent(Guid vehicleId)
        {
            var currentLocation = GeoService.Instance.CurrentDeviceLocation;
            var candidate = new Event
            {
                VehicleId = vehicleId,
                Id = Guid.NewGuid(),
                Odometer = GetMaxEventAttributeForVehicleId(vehicleId, o => o.Odometer) + 10,
                TimeStamp = DateTimeOffset.Now,
                Location = new EventGeoPosition(currentLocation.Coordinate.Point.Position.Latitude, currentLocation.Coordinate.Point.Position.Longitude)
            };
            EventPreparedObservable.Publish(candidate);
        }

        public List<Event> QueryAllEventsByVehicleId(Guid vehicleId)
        {
            if(allEvents != null && allEvents.Count > 0)
            {
                return allEvents.Where(x => x.VehicleId == vehicleId).OrderBy(t => t.Odometer).Reverse().ToList();
            }
            return new List<Event>();
        }

        public async Task SaveAsync(List<Event> events)
        {
            if(events == null)
            {
                return;
            }
            foreach(var evt in events)
            {
                await SaveAsync(evt, false);
            }
            await PersistAsync();
        }


        public async Task SaveAsync(Event evt, bool doPersist = true)
        {
            OperationType op = OperationType.None;
            var current = allEvents.Where(x => x.Id == evt.Id).FirstOrDefault();
            var idx = allEvents.IndexOf(current);
            if (idx == -1)
            {
                allEvents.Add(evt);
                op = OperationType.Created;
            }
            else
            {
                allEvents[idx] = evt;
                op = OperationType.Updated;
            }

            if(doPersist)
            {
                await PersistAsync();
            }

            switch (op)
            {
                case OperationType.Created:
                    EventCreatedObservable.Publish(evt.Id);
                    break;
                case OperationType.Updated:
                    EventUpdatedObservable.Publish(evt.Id);
                    break;
                default:
                    throw new ArgumentException("Invalid operation type");
            }
        }

        public async Task DeleteAsync(Guid eventId)
        {
            var item = allEvents.ToList().FirstOrDefault(x => x.Id == eventId);
            if(item == null)
            {
                return;
            }

            foreach(var a in item.Attachments.ToList())
            {
                await DeleteAttachmentAsync(eventId, a.Id);
            }
         
            lock(eventLock)
            {
                allEvents.Remove(item);
            }

            await PersistAsync();
            EventDeletedObservable.Publish(eventId);
        }

        public async Task DeleteAllByVehicleIdAsync(Guid vehicleId)
        {
            var events = allEvents.Where(x => x.VehicleId == vehicleId).ToList();
            foreach(var evt in events)
            {
                var item = allEvents.ToList().FirstOrDefault(x => x.Id == evt.Id);
                foreach (var a in item.Attachments.ToList())
                {
                    await DeleteAttachmentAsync(evt.Id, a.Id, false);
                }
                allEvents.Remove(item);
                EventDeletedObservable.Publish(evt.Id);
            }
            await PersistAsync();
        }

        public async Task AddAttachmentAsync(StorageFile file, Guid eventId)
        {
            var evt = allEvents.Where(x => x.Id == eventId).FirstOrDefault();
            if(evt == null)
            {
                throw new ArgumentException("Invalid event id supplied");
            }
            var attachmentId = Guid.NewGuid();
            var newFilename = attachmentId.ToString() + file.FileType;
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            var exists = await roamingFolder.TryGetItemAsync(applicationAttachmentsPath);
            if(exists == null)
            {
                await roamingFolder.CreateFolderAsync(applicationAttachmentsPath);
            }
            var folder = await roamingFolder.GetFolderAsync(applicationAttachmentsPath);
            var res = await file.CopyAsync(folder, newFilename, NameCollisionOption.ReplaceExisting);
            var attachment = new Attachment
            {
                EventId = eventId,
                Id = attachmentId,
                FileName = file.Name,
                SourceFileName = newFilename
            };
            evt.Attachments.Add(attachment);
            await PersistAsync();
            AttachmentAddedObservable.Publish(attachment);
        }

        internal void CancelPrepareNewEvent()
        {
            EventPreparedCancelledObservable.Publish(Guid.Empty);
        }

        public async Task DeleteAttachmentAsync(Guid eventId, Guid attachmentId, bool doPersist = true)
        {
            var evt = QueryEventById(eventId);

            var attachment = evt.Attachments.Where(id => id.Id == attachmentId).FirstOrDefault();
            if (attachment == null)
            {
                throw new ArgumentException("Invalid attachment id supplied");
            }

            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            var filePath = Path.Combine(applicationAttachmentsPath, attachment.SourceFileName);
            StorageFile file = await roamingFolder.GetFileAsync(filePath);
            await file.DeleteAsync();
            evt.Attachments.Remove(attachment);
            if(doPersist)
            {
                await PersistAsync();
            }
            AttachmentDeletedObservable.Publish(attachment);
        }

        private void Load()
        {
            var container = new PersistentDataContainer<List<Event>>();
            Task.Run(async () =>
            {
                await container.LoadAsync(path);
            }).Wait();

            if (container.Data != null)
            {
                allEvents = container.Data;

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
            var container = new PersistentDataContainer<List<Event>>(allEvents, metadata);
            await container.PersistAsync(path);
        }
    }
}
