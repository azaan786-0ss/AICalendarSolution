namespace AICalendar.Domain.Entities
{
    public class Reminder
    {
        public int Id { get; set; }
        public DateTime ReminderTime { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        public string ClientReferenceId { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Attendees { get; set; }
    }
}
