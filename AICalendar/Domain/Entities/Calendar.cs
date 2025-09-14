namespace AICalendar.Domain.Entities
{
    public class Calendar
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
