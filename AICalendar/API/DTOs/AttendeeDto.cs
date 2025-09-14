using System.ComponentModel.DataAnnotations;

namespace AICalendar.DTOs
{
    public class AttendeeDto
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression("PENDING|ACCEPTED|DECLINED|TENTATIVE", ErrorMessage = "Status must be PENDING, ACCEPTED, DECLINED, or TENTATIVE.")]
        public string Status { get; set; } = "PENDING";
    }
}