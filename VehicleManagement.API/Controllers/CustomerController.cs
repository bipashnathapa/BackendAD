using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehicle.Domain.Models;

namespace VehicleManagement.Controllers;

public class CustomerController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    [HttpGet("/customer")]
    public IActionResult Index() => RedirectToAction("Dashboard");

    [HttpGet("/customer/dashboard")]
    public IActionResult Dashboard() => View("CustomerDashboard");

    [HttpGet("/customer/settings")]
    public IActionResult Settings() => View();

    [HttpGet("/customer/profile")]
    public IActionResult Profile() => View("Profile");

    [HttpPost("/customer/profile")]
    public async Task<IActionResult> UpdateProfile(string FullName, string Email, string Phone, string Address, DateTime? DOB)
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user == null)
        {
            user = await _userManager.Users.FirstOrDefaultAsync();
        }

        if (user == null)
        {
            TempData["ErrorMessage"] = "No user found in the database to update.";
            return RedirectToAction("Profile");
        }

        // Update basic properties
        user.FullName = FullName;
        user.PhoneNumber = Phone;
        user.Address = Address;
        
        if (DOB.HasValue)
        {
            // PostgreSQL requires UTC for 'timestamp with time zone'
            user.DOB = DateTime.SpecifyKind(DOB.Value, DateTimeKind.Utc);
        }

        // Update Email and UserName (keeping them in sync for this app)
        if (!string.IsNullOrEmpty(Email) && user.Email != Email)
        {
            user.Email = Email;
            user.UserName = Email;
            user.NormalizedEmail = Email.ToUpper();
            user.NormalizedUserName = Email.ToUpper();
        }

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Profile updated successfully!";
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            TempData["ErrorMessage"] = $"Failed to update profile: {errors}";
        }

        return RedirectToAction("Profile");
    }
}

