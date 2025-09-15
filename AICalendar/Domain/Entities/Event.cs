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
        public string ClientReferenceId { get; set; }
        public string Timezone { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public string AttendeesRaw { get; set; }
    }
}
