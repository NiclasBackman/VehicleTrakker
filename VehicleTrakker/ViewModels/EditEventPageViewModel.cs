using AsyncAwaitBestPractices.MVVM;
using NavigationTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.Pages;
using Windows.UI.Xaml.Controls;

namespace VehicleTrakker.ViewModels
{
    public class EditEventPageViewModel
    {
        private readonly EditEventPage parent;

        public EditEventPageViewModel(EditEventPage parent)
        {
            GoBackCommand = new AsyncCommand(HandleGoBackClicked);
            this.parent = parent;
            var v = EventService.Instance.EventDeletedObservable.Subscribe(HandleEventDeleted);
        }

        private void HandleEventDeleted(Guid obj)
        {
            parent.Frame.Navigate(typeof(EventsPage));
        }

        private async Task HandleGoBackClicked()
        {
            var viewModel = parent as EditEventPage;
            if (viewModel.CanSaveEvent(Guid.Empty))
            {
                var dlgHelper = new DialogHelper();
                var res = await dlgHelper.DisplayMessageQuestionAsync("Save event", "You are about to navigate away and discard valid data, do you want to proceed?");
                if (res == DialogHelper.AnswerType.No)
                {
                    return;
                }
            }

            parent.Frame.Navigate(typeof(EventsPage));
        }

        public IAsyncCommand GoBackCommand { get; }

    }
}
