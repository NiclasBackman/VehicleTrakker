namespace VehicleTrakker.Services
{
    public class StorageMetaData
    {
        public StorageMetaData()
        {
        }

        public StorageMetaData(int version, string name)
        {
            Version = version;
            Name = name;
        }

        public int Version { get; set; }

        public string Name { get; set; }
    }
}
