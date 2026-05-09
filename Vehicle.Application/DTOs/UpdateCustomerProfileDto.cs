using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class UpdateCustomerProfileDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime? Dob { get; set; }
}
