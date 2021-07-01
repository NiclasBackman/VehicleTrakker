using AsyncAwaitBestPractices.MVVM;
using NavigationTest;
using NavigationTest.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Popups;

namespace VehicleTrakker.ViewModels
{
    public class AttachmentViewModel
    {
        public AttachmentViewModel()
        {
            OpenAttachmentCommand = new AsyncCommand(HandleOpenAttachmentClicked, CanExecuteOpenAttachment);
            DeleteAttachmentCommand = new AsyncCommand(HandleDeleteAttachmentClicked, CanExecuteDeleteAttachment);
            dialogService = new DialogHelper();
        }

        private async Task HandleDeleteAttachmentClicked()
        {
            var res = await dialogService.DisplayMessageQuestionAsync("Delete attachment [" + FileName + "]",
                                                                      "You are about to delete the attachment, are you sure you want to proceed?");
            if(res == DialogHelper.AnswerType.No)
            {
                return;
            }

            await EventService.Instance.DeleteAttachmentAsync(EventId, Id);
        }

        private bool CanExecuteOpenAttachment(object arg)
        {
            return true;
        }

        private bool CanExecuteDeleteAttachment(object arg)
        {
            return true;
        }

        public IAsyncCommand OpenAttachmentCommand { get; }

        public IAsyncCommand DeleteAttachmentCommand { get; }

        private DialogHelper dialogService;

        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public string FileName { get; set; }

        public string SourceFileName { get; set; }

        private async Task HandleOpenAttachmentClicked()
        {
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            StorageFile file = await roamingFolder.GetFileAsync(Path.Combine("ZalcinSoft", "VehicleTrakker", "Attachments", SourceFileName));
            await Windows.System.Launcher.LaunchFileAsync(file);
        }
    }
}
