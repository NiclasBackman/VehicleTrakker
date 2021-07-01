using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace VehicleTrakker
{
    public class DialogHelper
    {
        public enum AnswerType
        {
            Yes = 0, 
            No
        }

        public async Task<AnswerType> DisplayMessageQuestionAsync(String title, String content)
        {
            var messageDialog = new MessageDialog(content, title);
            messageDialog.Commands.Add(new UICommand(AnswerType.Yes.ToString(), null));
            messageDialog.Commands.Add(new UICommand(AnswerType.No.ToString(), null));
            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;
            var cmdResult = await messageDialog.ShowAsync();
            var e = Enum.Parse(typeof(AnswerType), cmdResult.Label);
            return (AnswerType)e;
        }

        public async Task DisplayMessageAsync(String title, String content)
        {

            ContentDialog dlg = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };

            await dlg.ShowAsync();
        }
    }
}
