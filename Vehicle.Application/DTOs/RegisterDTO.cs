using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class RegisterDTO
{
    [Required]
    public required string FullName { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(6)]
    public required string Password { get; set; }

    [Required]
    public required string Address { get; set; }

    [Required]
    public required string UserRole { get; set; } // Should be "Customer" or "Staff"
}