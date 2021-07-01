using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls.Maps;

namespace VehicleTrakker.ViewModels
{
    public class MapStyleImageMapping
    {
        public static ObservableCollection<MapStyleImageMapping> AllMapStyles = new ObservableCollection<MapStyleImageMapping>()
        {
            //new MapStyleImageMapping(MapStyle.None, string.Empty),
            new MapStyleImageMapping(MapStyle.Road, "/Images/Map/road.png"),
            new MapStyleImageMapping(MapStyle.Aerial, "/Images/Map/aerial.png"),
            new MapStyleImageMapping(MapStyle.AerialWithRoads, "/Images/Map/aerial_with_roads.png"),
            new MapStyleImageMapping(MapStyle.Terrain, "/Images/Map/terrain.png"),
            new MapStyleImageMapping(MapStyle.Aerial3D, "/Images/Map/aerial_3D.png"),
            new MapStyleImageMapping(MapStyle.Aerial3DWithRoads, "/Images/Map/aerial_3D_with_roads.png")
        };

        public MapStyleImageMapping(MapStyle type, string imgSource)
        {
            Type = type;
            ImageSource = imgSource;
        }

        public MapStyle Type { get; }

        public string ImageSource { get; }
    }
}
