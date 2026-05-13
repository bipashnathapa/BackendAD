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

    [HttpGet("/staff/new-sale")]
    public IActionResult NewSale() => View();

    [HttpGet("/staff/invoice/{id:int}")]
    public IActionResult Invoice(int id)
    {
        ViewData["InvoiceId"] = id;
        return View();
    }

    [HttpGet("/staff/reports")]
    public IActionResult Reports() => View();

    [HttpGet("/staff/low-stock")]
    public IActionResult LowStock() => View();

    [HttpGet("/staff/service-requests")]
    public IActionResult ServiceRequests() => View();

    [HttpGet("/staff/customers/{id:int}")]
    public IActionResult CustomerHistory(int id)
    {
        ViewData["CustomerId"] = id;
        return View("CustomerHistory");
    }
}
