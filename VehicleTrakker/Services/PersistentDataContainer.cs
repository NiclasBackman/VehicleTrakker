using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace VehicleTrakker.Services
{
    public class PersistentDataContainer<T>
    {
        public PersistentDataContainer()
        {
        }

        public PersistentDataContainer(T data, StorageMetaData metaData)
        {
            Data = data;
            MetaData = metaData;
        }

        public T Data { get; set; }

        public StorageMetaData MetaData { get; set; }

        public async Task PersistAsync(string path)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string json = JsonConvert.SerializeObject(this, settings);
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            var file = await roamingFolder.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, json);
        }

        public async Task LoadAsync(string path)
        {
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;

            var exists = await roamingFolder.TryGetItemAsync(path);
            if (exists != null)
            {
                StorageFile file = await roamingFolder.GetFileAsync(path);
                string json = await FileIO.ReadTextAsync(file);
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                try
                {

                    var data = JsonConvert.DeserializeObject<PersistentDataContainer<T>>(json, settings);
                    this.Data = data.Data;
                    this.MetaData = data.MetaData;
                }
                catch (JsonSerializationException)
                {
                    // Currently doing nothing
                }
            }
        }
    }
}
