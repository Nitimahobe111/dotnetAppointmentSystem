using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class AppointmentModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? PatientId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        public DateTime EndTime   {get; set; }
        public string? AppointmentStatus { get; set; }

    }
}
