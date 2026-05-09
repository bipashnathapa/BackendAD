using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("/customer/invoices")]
    public IActionResult Invoices() => View();

    [HttpGet("/customer/profile")]
    public IActionResult Profile() => View("Profile");
}
