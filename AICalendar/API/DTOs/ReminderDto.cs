using System.ComponentModel.DataAnnotations;

namespace AICalendar.DTOs
{
    public class ReminderDto
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReminderTime { get; set; }
    }
}