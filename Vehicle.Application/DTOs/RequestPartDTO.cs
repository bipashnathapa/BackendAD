using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class RequestPartDTO
{
    public int? VehicleId { get; set; }

    [Required]
    [MaxLength(120)]
    public string PartName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string? VehicleModel { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; } = 1;

    [MaxLength(500)]
    public string? Notes { get; set; }
}
