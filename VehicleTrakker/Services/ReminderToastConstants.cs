using System;

namespace VehicleTrakker.Services
{
    public sealed class ReminderToastConstants
    {
        public const string Item1Day = "Item1Day";
        public const string Item1Week = "Item1Week";
        public const string Item2Weeks = "Item2Weeks";
        public const string Item1Month = "Item1Month";
        public const string ReminderIsExpiredAction = "ReminderIsExpired";
        public const string ReminderButtonAction = "ReminderButtonAction";
        public const string ReminderButtonActionConfirmed = "confirmed";
        public const string ReminderButtonActionPostpond = "postpond";
        public const string ReminderId = "ReminderId";
        public const string PostpondValueSelection = "PostpondValueSelection";
        
        public static int ItemTagToNumberOfDays(string tag)
        {
            switch(tag)
            {
                case Item1Day:
                    return 1;
                case Item1Week:
                    return 7;
                case Item2Weeks:
                    return 14;
                case Item1Month:
                    return 30;
                default:
                    throw new ArgumentException("Invalid tag: " + tag);
            }
        }
    }
}
