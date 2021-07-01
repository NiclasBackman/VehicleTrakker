using System;

namespace NavigationTest.ViewModels
{
    class TollFeeUserControlViewModel : BindableBase
    {
        public TollFeeUserControlViewModel() : base()
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

