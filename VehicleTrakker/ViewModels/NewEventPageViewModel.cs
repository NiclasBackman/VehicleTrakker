using AsyncAwaitBestPractices.MVVM;
using NavigationTest;
using NavigationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTrakker.Pages;
using Windows.UI.Xaml.Controls;

namespace VehicleTrakker.ViewModels
{
    class NewEventPageViewModel
    {
        private readonly Page parent;

        public NewEventPageViewModel(Page parent)
        {
            GoBackCommand = new AsyncCommand(HandleGoBackClicked);
            this.parent = parent;
        }

        private async Task HandleGoBackClicked()
        {
            var viewModel = parent as NewEventPage;
            if(viewModel.CanSaveEvent(Guid.Empty))
            {
                var dlgHelper = new DialogHelper();
                var res = await dlgHelper.DisplayMessageQuestionAsync("Save new event", "You are about to navigate away and discard valid data, do you want to proceed?");
                if(res == DialogHelper.AnswerType.No)
                {
                    return;
                }
            }

            parent.Frame.Navigate(typeof(EventsPage));
        }

        public IAsyncCommand GoBackCommand { get; }
    }
}
