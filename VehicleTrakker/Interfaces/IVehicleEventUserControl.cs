using System;
using VehicleTrakker.DataDefinitions;

namespace VehicleTrakker.Interfaces
{
    interface IVehicleEventUserControl
    {
        bool HasValidData();

        bool IsDirty(Guid eventId);

        void ClearData();

        Event GetData();

        void Update(Event evt);
    }
}
