using Microsoft.AspNetCore.Identity;

namespace Vehicle.Domain.Models;


public class ApplicationUser : IdentityUser
{
    
    public string FullName { get; set; }
    public string? Address { get; set; }
    
    
    public string UserRole { get; set; } 
}