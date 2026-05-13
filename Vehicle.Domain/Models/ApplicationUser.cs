using Microsoft.AspNetCore.Identity;

namespace Vehicle.Domain.Models;


public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = default!;
    public string? Address { get; set; }
    public DateTime? DOB { get; set; }

    public string UserRole { get; set; } = default!;
}
