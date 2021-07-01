using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;

namespace Sales_Dashboard
{
    public class Data : ViewModelBase//INotifyPropertyChanged
    {
        public List<SubData> ChartItems { get; set; }
        public string Category { get; set; }
        public double Value { get; set; }
        public string Price { get; set; }
        public double Percent { get; set; }
        public string Type { get; set; }
        public string Order { get; set; }
        public string Date { get; set; }
        public string ItemSold { get; set; }
        public string Country { get; set; }
        public string Amount { get; set; }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged();
                }
            }
        }



        //public event PropertyChangedEventHandler PropertyChanged;

        //private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        //{
        //    var handler = this.PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }

    public class SubData
    {
        public DateTime Category { get; set; }
        public double Value { get; set; }

    }
}
