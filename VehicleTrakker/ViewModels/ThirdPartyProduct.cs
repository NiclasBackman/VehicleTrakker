namespace VehicleTrakker.ViewModels
{
    public class ThirdPartyProduct
    {
        public ThirdPartyProduct(string name, string version, string url)
        {
            Name = name;
            Version = version;
            Url = url;
        }

        public string Name { get; }

        public string Version { get; }

        public string Url { get; }
    }
}