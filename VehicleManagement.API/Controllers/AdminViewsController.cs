using Microsoft.AspNetCore.Mvc;

namespace VehicleManagement.Controllers;

public class AdminViewsController : Controller
{
    [HttpGet("/admin")]            public IActionResult Index() => Redirect("/admin/dashboard");
    [HttpGet("/admin/dashboard")]  public IActionResult Dashboard() => View();
    [HttpGet("/admin/vendors")]    public IActionResult Vendors() => View();
    [HttpGet("/admin/parts")]      public IActionResult Parts() => View();
    [HttpGet("/admin/low-stock")]  public IActionResult LowStock() => View();
    [HttpGet("/admin/reports")]    public IActionResult Reports() => View();
}
