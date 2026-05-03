using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class BookAppointmentDTO
{
    [Required]
    public int VehicleId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string ServiceType { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Notes { get; set; }
}
