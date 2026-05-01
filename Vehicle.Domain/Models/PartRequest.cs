using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle.Domain.Models;

public class PartRequest
{
    [Key]
    public int PartRequestID { get; set; }

    public int CustomerID { get; set; }

    [ForeignKey("CustomerID")]
    public Customer Customer { get; set; } = null!;

    public int? VehicleID { get; set; }

    [ForeignKey("VehicleID")]
    public VehicleInfo? Vehicle { get; set; }

    [Required]
    [MaxLength(120)]
    public string PartName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string? VehicleModel { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; } = 1;

    [MaxLength(500)]
    public string? Notes { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Requested";

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}
