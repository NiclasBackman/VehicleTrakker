using Microsoft.Toolkit.Uwp.Notifications;
using NavigationTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;
using Windows.UI.Core;
using static VehicleTrakker.DataDefinitions.Reminder;

namespace VehicleTrakker.Services
{
    public sealed class ReminderService
    {
        private List<Reminder> allReminders;
        private bool doPersist;
        private static readonly ReminderService instance = new ReminderService();
        private readonly string path = Path.Combine("ZalcinSoft", "VehicleTrakker", "reminders.json");
        private readonly ValueTuple<string, string>[] postpondAlternatives = new ValueTuple<string, string>[]
        {
            new ValueTuple<string, string>(ReminderToastConstants.Item1Day, "1 Day"),
            new ValueTuple<string, string>(ReminderToastConstants.Item1Week, "1 Week"),
            new ValueTuple<string, string>(ReminderToastConstants.Item2Weeks, "2 Weeks"),
            new ValueTuple<string, string>(ReminderToastConstants.Item1Month, "1 Month")
        };
        
        private const int DataVersion = 1;
        private StorageMetaData metadata = new StorageMetaData(DataVersion, "Reminders");

        static ReminderService()
        {
        }

        private ReminderService()
        {
            allReminders = new List<Reminder>();
            doPersist = true;
            Load();
            ReminderCreatedObservable = new ObservableProperty<Reminder>();
            ReminderUpdatedObservable = new ObservableProperty<Guid>();
            ReminderExpiredObservable = new ObservableProperty<Guid>();
            ReminderDeletedObservable = new ObservableProperty<Guid>();
        }

        public static ReminderService Instance
        {
            get
            {
                return instance;
            }
        }

        internal List<Reminder> QueryAllReminders()
        {
            return allReminders ?? new List<Reminder>();
        }

        public ObservableProperty<Guid> ReminderDeletedObservable
        {
            get;
        }

        public ObservableProperty<Reminder> ReminderCreatedObservable
        {
            get;
        }

        public ObservableProperty<Guid> ReminderUpdatedObservable
        {
            get;
        }

        public ObservableProperty<Guid> ReminderExpiredObservable
        {
            get;
        }

        public void StartImportTransaction()
        {
            doPersist = false;
        }

        public void DoneImportTransaction()
        {
            doPersist = true;
        }

        public async Task IndicateStateAsync(Guid id, Reminder.ReminderState state)
        {
            var reminder = QueryReminderById(id);
            string header = "Reminder is expired";
            if(reminder.VehicleId != Guid.Empty)
            {
                var vehicle = VehicleService.Instance.QueryVehicleById(reminder.VehicleId);
                header += " for vehicle '" + vehicle.Name + "'";
            }

            if (state == Reminder.ReminderState.Expired)
            {
                new ToastContentBuilder()
                    .AddArgument("action", ReminderToastConstants.ReminderIsExpiredAction)
                    .AddArgument(ReminderToastConstants.ReminderId, id.ToString())
                    .AddText(header)
                    .AddText(reminder.Message)
                    .AddButton(new ToastButton()
                    .SetContent("Confirm")
                    .AddArgument(ReminderToastConstants.ReminderButtonAction, ReminderToastConstants.ReminderButtonActionConfirmed))
                    .AddButton(new ToastButton()
                    .SetContent("Postpond")
                    .AddArgument(ReminderToastConstants.ReminderButtonAction, ReminderToastConstants.ReminderButtonActionPostpond))
                    .AddComboBox(ReminderToastConstants.PostpondValueSelection, "Postpond reminder", ReminderToastConstants.Item1Week, postpondAlternatives)
                    .Show();
            }
            reminder.State = state;
            await UpdateAsync(reminder);
        }


        public Reminder QueryReminderById(Guid reminderId)
        {
            return allReminders.Where(x => x.Id == reminderId).FirstOrDefault();
        }

        public async Task CreateAsync(string message, DateTimeOffset expires, Guid vehicleId)
        {
            var now = DateTimeOffset.Now;
            var reminder = new Reminder(expires, message, vehicleId)
            {
                CreationDate = now,
                Id = Guid.NewGuid(),
                State = ReminderState.Idle
            };
            allReminders.Add(reminder);
            await PersistAsync();
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
             () =>
             {
                 ReminderCreatedObservable.Publish(reminder);
             });
        }

        public async Task CreateAsync()
        {
            var now = DateTimeOffset.Now;
            var reminder = new Reminder(now.AddMonths(6), "Add message here...", Guid.Empty)
            {
                CreationDate = now,
                Id = Guid.NewGuid(),
                State = ReminderState.Idle
            };
            allReminders.Add(reminder);
            await PersistAsync();
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
             () =>
             {
                 ReminderCreatedObservable.Publish(reminder);
             });
        }

        public async Task UpdateAsync(Reminder r)
        {
            var item = allReminders.Where(x => x.Id == r.Id).FirstOrDefault();
            var idx = allReminders.IndexOf(item);
            if (r.ExpirationDate != item.ExpirationDate)
            {
                if(r.ExpirationDate > DateTimeOffset.Now)
                {
                    r.State = ReminderState.Idle;
                }
            }

            allReminders[idx] = r;
            await PersistAsync();
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
             () =>
             {
                 ReminderUpdatedObservable.Publish(r.Id);
             });
        }

        public async Task DeleteAsync(Guid reminderId, bool doPersist = true)
        {
            allReminders.RemoveAll(x => x.Id == reminderId);
            if(doPersist)
            {
                await PersistAsync();
            }
            ReminderDeletedObservable.Publish(reminderId);
        }

        private void Load()
        {
            var container = new PersistentDataContainer<List<Reminder>>();
            Task.Run(async () =>
            {
                await container.LoadAsync(path);
            }).Wait();

            if (container.Data != null)
            {
                allReminders = container.Data;

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

        public async Task PersistAsync()
        {
            if (doPersist)
            {
                var container = new PersistentDataContainer<List<Reminder>>(allReminders, metadata);
                await container.PersistAsync(path);
            }
        }
    }
}
