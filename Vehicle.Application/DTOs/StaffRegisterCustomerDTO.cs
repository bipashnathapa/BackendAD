using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class StaffRegisterCustomerDto
{
    [Required]
    public required string FullName { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(6)]
    public required string Password { get; set; }

    public string? Address { get; set; }

    // Initial vehicle details
    [Required]
    public required string VehicleNo { get; set; }

    [Required]
    public required string Brand { get; set; }

    public string? Model { get; set; }
    public string? Type { get; set; }
}

