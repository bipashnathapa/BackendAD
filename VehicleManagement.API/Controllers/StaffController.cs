using Microsoft.AspNetCore.Mvc;

namespace VehicleManagement.Controllers;

public class StaffController : Controller
{
    [HttpGet("/staff")]
    public IActionResult Index() => View();
}

