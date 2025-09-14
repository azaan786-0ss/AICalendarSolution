namespace AICalendar.Domain.Entities
{
    public class Attendee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        public string Status { get; set; } = "PENDING";
    }
}
