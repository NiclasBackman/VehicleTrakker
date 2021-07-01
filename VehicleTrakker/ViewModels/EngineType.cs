using System.ComponentModel;

namespace NavigationTest
{
    public enum EngineType
    {
        [Description("Internal Combustion Engine")]
        ICE,
        [Description("Battery Electric")]
        PureEv,
        [Description("Plugin Hybrid")]
        Hybrid
    }
}
