using Microsoft.AspNetCore.Mvc;

namespace VehicleManagement.Controllers;

public class CustomerController : Controller
{
    [HttpGet("/customer")]
    public IActionResult Index() => View();
}

