using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class CreateServiceReviewDTO
{
    public int? AppointmentId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [MaxLength(800)]
    public string Comment { get; set; } = string.Empty;
}
