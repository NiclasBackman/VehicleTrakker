using VehicleTrakker;

namespace NavigationTest
{
    public class EngineTypeEntry
    {
        public EngineTypeEntry(EngineType type)
        {
            EngineType = type;
        }

        public EngineType EngineType { get; set; }

        public string Description => EngineType.ToDescriptionString();
    }
}
