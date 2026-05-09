using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class VendorDTO
{
    public int VendorID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PanNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVendorDTO
{
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string ContactPerson { get; set; } = string.Empty;
    [Required, MaxLength(20)]  public string Phone { get; set; } = string.Empty;
    [EmailAddress, MaxLength(150)] public string? Email { get; set; }
    [MaxLength(300)] public string? Address { get; set; }
    [MaxLength(50)]  public string? PanNumber { get; set; }
}

public class UpdateVendorDTO : CreateVendorDTO
{
    public bool IsActive { get; set; } = true;
}
