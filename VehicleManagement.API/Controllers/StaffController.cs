using Microsoft.AspNetCore.Mvc;

namespace VehicleManagement.Controllers;

public class StaffController : Controller
{
    [HttpGet("/staff")]
    public IActionResult Index() => Redirect("/staff/dashboard");

    [HttpGet("/staff/register")]
    public IActionResult Register() => View("Index");

    [HttpGet("/staff/dashboard")]
    public IActionResult Dashboard() => View();
}

