using NavigationTest.ViewModels;
using System;

namespace VehicleTrakker.ViewModels
{
    public class TaxUserControlViewModel : BindableBase
    {
        public TaxUserControlViewModel() : base()
        {
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
            return false;
        }
    }
}
