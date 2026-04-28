using Microsoft.AspNetCore.Mvc;

namespace VehicleManagement.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index() => View();
}

