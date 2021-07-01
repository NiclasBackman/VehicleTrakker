using VehicleTrakker;
using static VehicleTrakker.DataDefinitions.Vehicle;

namespace NavigationTest
{
    public class ServiceInterval
    {
        public ServiceInterval(ServiceIntervalType type)
        {
            Type = type;
            FriendlyName = type.ToDescriptionString();
        }

        public ServiceIntervalType Type { get; }

        public string FriendlyName { get; }
    }
}
