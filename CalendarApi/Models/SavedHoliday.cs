using System.ComponentModel.DataAnnotations;

namespace CalendarApi.Models
{
    public class SavedHoliday
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date must be in yyyy-MM-dd format.")]
        public string Date { get; set; } = string.Empty;

        [Required(ErrorMessage = "EventName is required.")]
        [StringLength(100, ErrorMessage = "EventName cannot exceed 100 characters.")]
        public string EventName { get; set; } = string.Empty;
    }
}