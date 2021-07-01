using NavigationTest;
using NavigationTest.ViewModels;
using System;
using System.ComponentModel;

namespace VehicleTrakker.DataDefinitions
{
    public class Vehicle
    {
        public enum ServiceIntervalType
        {
            [Description("Every 6:th month")]
            Every6Month = 0,
            [Description("Every 12:th month")]
            Every12Month,
            [Description("Every 18:th month")]
            Every18Month,
            [Description("Every 24:th month")]
            Every24Month
        }

        public Vehicle()
        {
        }

        //public Vehicle(Vehicle v)
        //{
        //    Name = v.Name;
        //    RegistrationNumber = v.RegistrationNumber;
        //    DateOfPurchase = v.DateOfPurchase;
        //    Brand = v.Brand;
        //    Id = v.Id;
        //    IsFavorite = v.IsFavorite;
        //    ServiceInterval = v.ServiceInterval;
        //    ServiceDistance = v.ServiceDistance;
        //    EngineType = v.EngineType;
        //}

        public Vehicle(string name, string registrationNumber, DateTime dateOfPurchase, string brand, Guid id, bool isFavorite,
                        ServiceIntervalType serviceIntervalType, int serviceDistance, EngineType engineType)
        {
            Name = name;
            RegistrationNumber = registrationNumber;
            DateOfPurchase = dateOfPurchase;
            Brand = brand;
            Id = id;
            IsFavorite = isFavorite;
            ServiceInterval = serviceIntervalType;
            ServiceDistance = serviceDistance;
            EngineType = engineType;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string RegistrationNumber { get; set; }

        public DateTime DateOfPurchase { get; set; }

        public string Brand { get; set; }

        public bool IsFavorite { get; set; }

        public ServiceIntervalType ServiceInterval { get; set; }

        public EngineType EngineType { get; set; }

        public int ServiceDistance { get; set; } 
    }
}