namespace AICalendar.Domain.Entities
{
    public class Reminder
    {
        public int Id { get; set; }
        public DateTime ReminderTime { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
    }
}
