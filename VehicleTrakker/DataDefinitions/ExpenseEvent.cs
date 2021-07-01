namespace VehicleTrakker.DataDefinitions
{
    public class ExpenseEvent : Event
    {
        public ExpenseEvent() : base()
        {
            this.Type = EventType.Expense;
        }
    }
}
