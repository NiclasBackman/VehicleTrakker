using NavigationTest;
using System.ComponentModel;
using VehicleTrakker.DataDefinitions;
using VehicleTrakker.ViewModels;
using static VehicleTrakker.DataDefinitions.Settings;
using static VehicleTrakker.DataDefinitions.Vehicle;

namespace VehicleTrakker
{
    public static class EnumExtension
    {
        public static string ToDescriptionString(this EngineType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static string ToDescriptionString(this ActionType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static string ToDescriptionString(this InspectionResultType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static string ToDescriptionString(this FuelConsumptionType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static string ToDescriptionString(this ServiceIntervalType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static int ToNumberOfMonths(this ServiceIntervalType val)
        {
            switch(val)
            {
                case ServiceIntervalType.Every6Month:
                    return 6;
                case ServiceIntervalType.Every12Month:
                    return 12;
                case ServiceIntervalType.Every18Month:
                    return 18;
                case ServiceIntervalType.Every24Month:
                    return 24;
                default:
                    throw new InvalidEnumArgumentException("Unhandld enumm type: " + val);
            }
        }

        public static string ToDescriptionString(this EventTypeSelectionVisualizationType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static string ToDescriptionString(this EventType eventType)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])eventType
               .GetType()
               .GetField(eventType.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
