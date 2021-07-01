using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls.Maps;

namespace VehicleTrakker.ViewModels
{
    public class MapProjectionImageMapping
    {
        public static ObservableCollection<MapProjectionImageMapping> AllMapProjections = new ObservableCollection<MapProjectionImageMapping>()
        {
            new MapProjectionImageMapping(MapProjection.Globe, "/Images/Map/globe_projection.png"),
            new MapProjectionImageMapping(MapProjection.WebMercator, "/Images/Map/mercator_projection.png")
        };

        public MapProjectionImageMapping(MapProjection type, string imgSource)
        {
            Type = type;
            ImageSource = imgSource;
        }

        public MapProjection Type { get; }

        public string ImageSource { get; }
    }
}
