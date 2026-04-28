using Microsoft.AspNetCore.Mvc;

namespace VehicleManagement.Controllers;

public class AccountController : Controller
{
    [HttpGet("/login")]
    public IActionResult Login() => View();

    [HttpGet("/register")]
    public IActionResult Register() => View();
}

