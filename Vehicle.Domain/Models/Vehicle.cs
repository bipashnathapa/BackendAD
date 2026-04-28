using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle.Domain.Models;

public class VehicleInfo
{
    [Key]
    public int VehicleID { get; set; }

    [Required]
    public string VehicleNo { get; set; }

    [Required] // Added this to ensure every vehicle has a brand
    public string Brand { get; set; }

    public string Model { get; set; }
    public string Type { get; set; }

    public int CustomerID { get; set; }
    
    [ForeignKey("CustomerID")]
    public Customer Customer { get; set; }
}