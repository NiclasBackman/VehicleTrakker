using NavigationTest;
using NavigationTest.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using VehicleTrakker.DataDefinitions;

namespace VehicleTrakker.ViewModels
{
    public enum InspectionResultType
    {
        [Description("Passed")]
        Passed = 0,
        [Description("Passed with remarks")]
        PassedWithRemarks,
        [Description("Failed")]
        Failed
    }
    public class InspectionResult
    {
        public static ObservableCollection<InspectionResult> AllInspectionResults = new ObservableCollection<InspectionResult>()
        {
            new InspectionResult("/Images/Inspection/success.png", InspectionResultType.Passed),
            new InspectionResult("/Images/Inspection/warning.png", InspectionResultType.PassedWithRemarks),
            new InspectionResult("/Images/Inspection/fail.png", InspectionResultType.Failed)
        };

        public InspectionResult(string image, InspectionResultType result)
        {
            Result = result;
            Image = image;
        }

        public InspectionResultType Result { get; }

        public string ResultDescription => Result.ToDescriptionString();

        public string Image { get; }
    }


    class InspectionUserControlViewModel : BindableBase
    {
        private InspectionResult selectedResult;

        public InspectionUserControlViewModel() : base()
        {
            InspectionResults = InspectionResult.AllInspectionResults;
            SelectedResult = InspectionResults.FirstOrDefault();
        }

        public ObservableCollection<InspectionResult> InspectionResults { get; }

        public InspectionResult SelectedResult
        {
            get { return selectedResult; }
            set
            {
                selectedResult = value;
                OnPropertyChanged("SelectedResult");
            }
        }
        

        internal bool HasValidData()
        {
            return true;
        }

        internal void ClearData()
        {
        }

        internal bool IsDirty(Guid eventId)
        {
            var evt = EventService.Instance.QueryEventById(eventId);
            if (evt == null)
            {
                return false;
            }
            var inspectionEvent = evt as InspectionEvent;
            if (inspectionEvent.Result == SelectedResult.Result)
            {
                return false;
            }
            return true;
        }
    }
}
