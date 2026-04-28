using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class RegisterDTO
{
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string UserRole { get; set; } // Should be "Customer" or "Staff"
}