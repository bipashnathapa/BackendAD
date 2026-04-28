using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle.Domain.Models;

public class Customer
{
    [Key]
    public int CustomerID { get; set; }

    
    public string UserID { get; set; }
    
    [ForeignKey("UserID")]
    public ApplicationUser User { get; set; }

    
    public ICollection<VehicleInfo> Vehicles { get; set; }
}