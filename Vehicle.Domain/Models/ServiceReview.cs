using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle.Domain.Models;

public class ServiceReview
{
    [Key]
    public int ServiceReviewID { get; set; }

    public int CustomerID { get; set; }

    [ForeignKey("CustomerID")]
    public Customer Customer { get; set; } = null!;

    public int? AppointmentID { get; set; }

    [ForeignKey("AppointmentID")]
    public Appointment? Appointment { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [MaxLength(800)]
    public string Comment { get; set; } = string.Empty;

    public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;
}
