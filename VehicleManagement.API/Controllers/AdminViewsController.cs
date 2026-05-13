using Microsoft.AspNetCore.Mvc;

namespace VehicleManagement.Controllers;

public class AdminViewsController : Controller
{
    [HttpGet("/admin")]            public IActionResult Index() => Redirect("/admin/dashboard");
    [HttpGet("/admin/dashboard")]  public IActionResult Dashboard() => View("~/Views/Admin/Dashboard.cshtml");
    [HttpGet("/admin/vendors")]    public IActionResult Vendors() => View("~/Views/Admin/Vendors.cshtml");
    [HttpGet("/admin/parts")]      public IActionResult Parts() => View("~/Views/Admin/Parts.cshtml");
    [HttpGet("/admin/low-stock")]  public IActionResult LowStock() => View("~/Views/Admin/LowStock.cshtml");
    [HttpGet("/admin/reports")]    public IActionResult Reports() => View("~/Views/Admin/Reports.cshtml");
}
