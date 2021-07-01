using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static VehicleTrakker.DataDefinitions.Reminder;

namespace VehicleTrakker.Services
{
    public class ReminderChecker
    {
        public static void DoWork()
        {
            var reminderService = ReminderService.Instance;
            for (; ; )
            {
                Console.WriteLine("Working thread...");
                var now = DateTimeOffset.Now;
                foreach(var reminder in reminderService.QueryAllReminders()?.Where(x => x.State == ReminderState.Idle))
                {
                    if(reminder.ExpirationDate < now)
                    {
                        Task.Run(async () => { 
                            await reminderService.IndicateStateAsync(reminder.Id, ReminderState.Expired); 
                        });                        
                    }
                }
                Thread.Sleep(1000 * 60); // 1 min
            }
        }
    }
}
