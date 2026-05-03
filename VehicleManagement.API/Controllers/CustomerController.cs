using Microsoft.AspNetCore.Mvc;

namespace VehicleManagement.Controllers;

public class CustomerController : Controller
{
    [HttpGet("/customer")]
    public IActionResult Index() => RedirectToAction("Dashboard");

    [HttpGet("/customer/dashboard")]
    public IActionResult Dashboard() => View("CustomerDashboard");

    [HttpGet("/customer/history")]
    public IActionResult History() => View();

    [HttpGet("/customer/settings")]
    public IActionResult Settings() => View();
}

