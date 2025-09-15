namespace AICalendar.Domain.Entities
{
    public partial class Calendar
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Correct navigation property for EF Core
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }

    // Add this class for MCP event handling if not already present in your domain
    public class CalendarEvent
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Timezone { get; set; }
        public string Location { get; set; }
        public string Attendees { get; set; }
        public string Notes { get; set; }
        public string ClientReferenceId { get; set; }
    }
}
