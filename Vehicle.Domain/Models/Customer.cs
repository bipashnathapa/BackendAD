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

    
    public ICollection<VehicleInfo> Vehicles { get; set; } = new List<VehicleInfo>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<PartRequest> PartRequests { get; set; } = new List<PartRequest>();
    public ICollection<ServiceReview> ServiceReviews { get; set; } = new List<ServiceReview>();
}
