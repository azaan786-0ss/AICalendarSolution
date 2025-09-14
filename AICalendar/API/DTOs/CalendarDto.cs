using System.ComponentModel.DataAnnotations;

namespace AICalendar.DTOs
{
    public class CalendarDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
