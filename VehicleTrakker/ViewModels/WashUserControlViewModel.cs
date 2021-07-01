using NavigationTest.ViewModels;
using System;

namespace VehicleTrakker.ViewModels
{
    public class WashUserControlViewModel : BindableBase
    {
        public WashUserControlViewModel() : base()
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
