namespace AICalendar.Domain.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int CalendarId { get; set; }
        public Calendar Calendar { get; set; } = null!;
        public ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();
        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    }
}
