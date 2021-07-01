using NavigationTest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VehicleTrakker.DataDefinitions;
using Windows.Storage;

namespace VehicleTrakker.Services
{
    public sealed class ApplicationService
    {
        private static readonly ApplicationService instance = new ApplicationService();
        private readonly VehicleService vehicleService;
        private readonly EventService eventService;
        private readonly ReminderService reminderService;
        private readonly SettingsService settingsService;

        static ApplicationService()
        {
        }

        private ApplicationService()
        {
            vehicleService = VehicleService.Instance;
            eventService = EventService.Instance;
            reminderService = ReminderService.Instance;
            settingsService = SettingsService.Instance;

            vehicleService.VehicleDeletedObservable.Subscribe(HandleVehicleDeleted);
            eventService.EventCreatedObservable.Subscribe(HandleEventCreated);
        }

        public static ApplicationService Instance
        {
            get
            {
                return instance;
            }
        }

        private void HandleEventCreated(Guid eventId)
        {
            var evt = eventService.QueryEventById(eventId);
            if (evt.Type == EventType.Insurance)
            {
                var vehicle = vehicleService.QueryVehicleById(evt.VehicleId);
                InsuranceEvent iEvt = evt as InsuranceEvent;
                Task.Run(async () =>
                {
                    var expires = iEvt.EndDate.AddDays(settingsService.QuerySettings().ReminderMargin * (-1));
                    if (expires < DateTimeOffset.Now)
                    {
                        expires = DateTimeOffset.Now.AddDays(1);
                    }
                    await reminderService.CreateAsync($"Insurance for your vehicle [{vehicle.Name}] expires {iEvt.EndDate.ToString("d", CultureInfo.CurrentCulture)}", expires, iEvt.VehicleId);
                });
            }
            else if (evt.Type == EventType.Service)
            {
                var vehicle = vehicleService.QueryVehicleById(evt.VehicleId);
                ServiceEvent sEvt = evt as ServiceEvent;
                Task.Run(async () =>
                {
                    var plannedDueDate = sEvt.TimeStamp.AddMonths(vehicle.ServiceInterval.ToNumberOfMonths());
                    var expires = plannedDueDate.AddDays(settingsService.QuerySettings().ReminderMargin * (-1));
                    if (expires < DateTimeOffset.Now)
                    {
                        expires = DateTimeOffset.Now.AddDays(1);
                    }
                    await reminderService.CreateAsync($"Next scheduled service for your vehicle [{vehicle.Name}] is planned to {plannedDueDate.ToString("d", CultureInfo.CurrentCulture)}", expires, sEvt.VehicleId);
                });
            }
        }

        private void HandleVehicleDeleted(Guid vehicleId)
        {
            var reminders = reminderService.QueryAllReminders().Where(x => x.VehicleId == vehicleId).ToList();
            var t = Task.Run(async () =>
            {
                foreach (var reminder in reminders)
                {
                    await reminderService.DeleteAsync(reminder.Id, false);
                }
                await eventService.DeleteAllByVehicleIdAsync(vehicleId);
            });
            t.ContinueWith(async x => 
            {
                await reminderService.PersistAsync(); 
            });
        }

        public async Task<string> ExportAsync(Guid vehicleId, StorageFile filePath)
        {
            try
            {
                var vehicle = vehicleService.QueryVehicleById(vehicleId);
                var events = eventService.QueryAllEventsByVehicleId(vehicleId);
                var data = new AggregatedVehicle(vehicle, events);
                XmlSerializer xs = new XmlSerializer(typeof(AggregatedVehicle));
                using (StringWriter textWriter = new StringWriter())
                {
                    xs.Serialize(textWriter, data);
                    await FileIO.WriteTextAsync(filePath, textWriter.ToString());
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


            return string.Empty;
        }

        public async Task<bool> ImportAsync(StorageFile filePath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(AggregatedVehicle));
            var stream = await filePath.OpenAsync(Windows.Storage.FileAccessMode.Read);
            ulong size = stream.Size;
            using (var inputStream = stream.GetInputStreamAt(0))
            {
                using (var dataReader = new Windows.Storage.Streams.DataReader(inputStream))
                {
                    uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
                    string text = dataReader.ReadString(numBytesLoaded);
                    using (TextReader reader = new StringReader(text))
                    {
                        var res = (AggregatedVehicle)xs.Deserialize(reader);
                        if(res != null)
                        {
                            res.Vehicle.Id = Guid.NewGuid();
                            res.Vehicle.IsFavorite = vehicleService.QueryAllVehicles().Count() == 0;
                            await vehicleService.SaveAsync(res.Vehicle);
                            var eventCandidates = new List<Event>();
                            foreach(var evt in res.EventList)
                            {
                                evt.Id = Guid.NewGuid();
                                evt.VehicleId = res.Vehicle.Id;
                                evt.Attachments = new List<DataDefinitions.Attachment>();
                                eventCandidates.Add(evt);
                            }
                            ReminderService.Instance.StartImportTransaction();
                            await eventService.SaveAsync(eventCandidates);
                            ReminderService.Instance.DoneImportTransaction();
                            await ReminderService.Instance.PersistAsync();
                        }
                    }
                }
            }

            return true;
        }
    }
}
