using System.ComponentModel;
using System.Runtime.CompilerServices;
using VehicleTrakker.Services;

namespace UserControlTest
{
    public class DistanceAttributeUserControlViewModel : INotifyPropertyChanged
    {
        private string unitName;
        private string attributeValue;
        private string placeHolderText;

        public DistanceAttributeUserControlViewModel(string attributeName)
        {
            var settings = SettingsService.Instance.QuerySettings();
            UnitName = settings.DistanceUnit;
            PlaceHolderText = "Enter '" + attributeName + "' value";
        }

        public string UnitName
        {
            get { return unitName; }
            set
            {
                unitName = value;
                OnPropertyChanged();
            }
        }

        public string AttributeValue
        {
            get { return attributeValue; }
            set
            {
                attributeValue = value;
                OnPropertyChanged();
            }
        }
        public string PlaceHolderText
        {
            get { return placeHolderText; }
            set
            {
                placeHolderText = value;
                OnPropertyChanged();
            }
        }

        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
