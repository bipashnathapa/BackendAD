using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

public class StaffController : Controller
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet("/staff")]
    public IActionResult Index() => Redirect("/staff/dashboard");

    [HttpGet("/staff/register")]
    public IActionResult Register() => View("Index");

    [HttpGet("/staff/dashboard")]
    public IActionResult Dashboard() => View();

    [HttpGet("/staff/customers")]
    public async Task<IActionResult> Customers()
    {
        var customers = await _staffService.SearchCustomersAsync("");
        return View("Customer", customers);
    }
}



