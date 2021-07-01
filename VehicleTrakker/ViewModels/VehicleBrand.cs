using System.Collections.ObjectModel;

namespace NavigationTest
{
    public class VehicleBrand
    {
        public static ObservableCollection<VehicleBrand> AllVehicleBrands = new ObservableCollection<VehicleBrand>()
        {
            new VehicleBrand("/Images/Brands/acura.png", "Acura"),
            new VehicleBrand("/Images/Brands/alfa_romeo.png", "Alfa Romeo"),
            new VehicleBrand("/Images/Brands/aston_martin.png", "Aston Martin"),
            new VehicleBrand("/Images/Brands/audi.png", "Audi"),
            new VehicleBrand("/Images/Brands/bmw.png", "BMW" ),
            new VehicleBrand("/Images/Brands/chevrolet.png", "Chevrolet" ),
            new VehicleBrand("/Images/Brands/citroen.png", "Citroen" ),
            new VehicleBrand("/Images/Brands/ducati.png", "Ducati" ),
            new VehicleBrand("/Images/Brands/ferrari.png", "Ferrari" ),
            new VehicleBrand("/Images/Brands/fiat.png", "Fiat" ),
            new VehicleBrand("/Images/Brands/ford.png", "Ford" ),
            new VehicleBrand("/Images/Brands/harley_davidson.png", "Harley Davidson" ),
            new VehicleBrand("/Images/Brands/honda.png", "Honda (Car)" ),
            new VehicleBrand("/Images/Brands/honda_motorcycle.png", "Honda (MC)" ),
            new VehicleBrand("/Images/Brands/hyundai.png", "Hyundai" ),
            new VehicleBrand("/Images/Brands/infiniti.png", "Infiniti" ),
            new VehicleBrand("/Images/Brands/jaguar.png", "Jaguar" ),
            new VehicleBrand("/Images/Brands/jeep.png", "Jeep" ),
            new VehicleBrand("/Images/Brands/kawasaki.png", "Kawasaki" ),
            new VehicleBrand("/Images/Brands/kia.png", "Kia" ),
            new VehicleBrand("/Images/Brands/ktm.png", "KTM" ),
            new VehicleBrand("/Images/Brands/lancia.png", "Lancia" ),
            new VehicleBrand("/Images/Brands/landrover.png", "Land Rover" ),
            new VehicleBrand("/Images/Brands/lexus.png", "Lexus" ),
            new VehicleBrand("/Images/Brands/mazda.png", "Mazda" ),
            new VehicleBrand("/Images/Brands/mercedes.png", "Mercedes Benz" ),
            new VehicleBrand("/Images/Brands/mg.png", "MG" ),
            new VehicleBrand("/Images/Brands/mini.png", "Mini" ),
            new VehicleBrand("/Images/Brands/mitsubishi.png", "Mitsubishi" ),
            new VehicleBrand("/Images/Brands/nissan.png", "Nissan" ),
            new VehicleBrand("/Images/Brands/opel.png", "Opel" ),
            new VehicleBrand("/Images/Brands/peugeot.png", "Peugeot" ),
            new VehicleBrand("/Images/Brands/porsche.png", "Porsche" ),
            new VehicleBrand("/Images/Brands/renault.png", "Renault" ),
            new VehicleBrand("/Images/Brands/seat.png", "Seat" ),
            new VehicleBrand("/Images/Brands/skoda.png", "Skoda" ),
            new VehicleBrand("/Images/Brands/subaru.png", "Subaru" ),
            new VehicleBrand("/Images/Brands/suzuki.png", "Suzuki" ),
            new VehicleBrand("/Images/Brands/toyota.png", "Toyota" ),
            new VehicleBrand("/Images/Brands/triumph.png", "Triumph" ),
            new VehicleBrand("/Images/Brands/volkswagen.png", "Volkswagen" ),
            new VehicleBrand("/Images/Brands/volvo.png", "Volvo" ),
            new VehicleBrand("/Images/Brands/yamaha.png", "Yamaha" ),
        };

        public VehicleBrand(string image, string name)
        {
            Name = name;
            Image = image;
        }

        public string Name { get; }

        public string Image { get; }
    }
}
