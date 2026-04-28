using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle.Domain.Models;

public class Staff
{
    [Key]
    public int StaffID { get; set; }

    public string UserID { get; set; }
    
    [ForeignKey("UserID")]
    public ApplicationUser User { get; set; }
}