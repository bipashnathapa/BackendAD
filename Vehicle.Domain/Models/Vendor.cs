using System.ComponentModel.DataAnnotations;

namespace Vehicle.Domain.Models;

public class Vendor
{
    [Key] public int VendorID { get; set; }

    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string ContactPerson { get; set; } = string.Empty;
    [Required, MaxLength(20)]  public string Phone { get; set; } = string.Empty;
    [MaxLength(150)] public string? Email { get; set; }
    [MaxLength(300)] public string? Address { get; set; }
    [MaxLength(50)]  public string? PanNumber { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Part> Parts { get; set; } = new List<Part>();
}
