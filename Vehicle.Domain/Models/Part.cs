using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle.Domain.Models;

public class Part
{
    [Key] public int PartID { get; set; }

    [Required, MaxLength(150)] public string PartName { get; set; } = string.Empty;
    [MaxLength(80)]  public string? PartCode { get; set; }
    [MaxLength(120)] public string? Compatibility { get; set; }

    [Column(TypeName = "numeric(10,2)")] public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; } = 10;

    public int? VendorID { get; set; }
    [ForeignKey("VendorID")] public Vendor? Vendor { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
