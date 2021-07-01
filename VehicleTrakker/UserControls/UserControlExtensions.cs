using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.NumberFormatting;

namespace VehicleTrakker.UserControls
{
    class UserControlExtensions
    {
        public void SetNumberBoxNumberFormatter(NumberBox element, double inc)
        {
            IncrementNumberRounder rounder = new IncrementNumberRounder();
            rounder.Increment = inc;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundUp;

            DecimalFormatter formatter = new DecimalFormatter();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 2;
            formatter.NumberRounder = rounder;
            element.NumberFormatter = formatter;
        }

    }
}
