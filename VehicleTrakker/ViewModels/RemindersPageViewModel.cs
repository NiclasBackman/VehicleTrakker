using AsyncAwaitBestPractices.MVVM;
using Microsoft.Toolkit.Uwp.UI;
using NavigationTest.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.Services;
using Windows.UI.Xaml.Data;

namespace VehicleTrakker.ViewModels
{
    public class RemindersPageViewModel : BindableBase
    {
        private readonly ReminderService reminderService;
        private readonly ObservableCollection<ReminderViewModel> reminders;
        private readonly AdvancedCollectionView cvs;

        public RemindersPageViewModel()
        {
            reminderService = ReminderService.Instance;
            reminderService.ReminderCreatedObservable.Subscribe(HandleReminderCreated);
            reminderService.ReminderUpdatedObservable.Subscribe(HandleReminderUpdated);
            reminderService.ReminderDeletedObservable.Subscribe(HandleReminderDeleted);

            reminders = new ObservableCollection<ReminderViewModel>();
            cvs = new AdvancedCollectionView(reminders, true);
            cvs.SortDescriptions.Add(new SortDescription(nameof(Reminder.CreationDate), SortDirection.Descending));
            AddReminderCommand = new AsyncCommand(HandleAddEventClicked, CanAddEvent);
            foreach(var reminder in reminderService.QueryAllReminders())
            {
                reminders.Add(new ReminderViewModel(reminder));
            }
        }

        private void HandleReminderUpdated(Guid reminderId)
        {
            var reminder = reminderService.QueryReminderById(reminderId);
            var item = reminders.Where(x => x.Id == reminderId).FirstOrDefault();
            item.State = reminder.State;
        }

        private void HandleReminderCreated(Reminder reminder)
        {
            reminders.Add(new ReminderViewModel(reminder));
        }

        private void HandleReminderDeleted(Guid reminderId)
        {
            var item = reminders.Where(x => x.Id == reminderId).FirstOrDefault();
            reminders.Remove(item);
        }

        private async Task HandleAddEventClicked()
        {
            await reminderService.CreateAsync();
        }

        private bool CanAddEvent(object arg)
        {
            return true;
        }

        public IAsyncCommand AddReminderCommand { get; }

        public ICollectionView Reminders => cvs;
    }
}
