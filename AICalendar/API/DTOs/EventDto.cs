using System.ComponentModel.DataAnnotations;

namespace AICalendar.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int CalendarId { get; set; }
    }
}
