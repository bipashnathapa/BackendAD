using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class StaffRegisterCustomerDto
{
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    public string? Address { get; set; }

    // Initial vehicle details
    [Required]
    public string VehicleNo { get; set; }

    [Required]
    public string Brand { get; set; }

    public string? Model { get; set; }
    public string? Type { get; set; }
}

