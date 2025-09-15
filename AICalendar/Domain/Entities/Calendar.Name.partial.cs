using AICalendar.Domain.Entities;

public partial class Calendar
{
    public int Id { get; set; }
    public string Name
    {
        get => _name;
        set => _name = value?.Trim();
    }
    private string _name;

    public ICollection<Event> Events { get; set; } = new List<Event>();
}