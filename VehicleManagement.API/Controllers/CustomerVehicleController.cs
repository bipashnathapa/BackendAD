using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[ApiController]
[Route("api/customer/vehicles")]
[Authorize(Roles = "Customer")] // Only Customers can touch this controller
public class CustomerVehicleController : ControllerBase
{
    private readonly ICustomerVehicleService _customerVehicleService;

    public CustomerVehicleController(ICustomerVehicleService customerVehicleService)
    {
        _customerVehicleService = customerVehicleService;
    }

    [HttpPost]
    public async Task<IActionResult> AddVehicle([FromBody] CustomerAddVehicleDTO dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var result = await _customerVehicleService.AddMyVehicleAsync(userIdStr, dto);
        
        return result ? Ok(new { message = "Vehicle successfully registered to your account!" }) : BadRequest();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMyVehicles()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var vehicles = await _customerVehicleService.GetMyVehiclesAsync(userIdStr);
        return Ok(vehicles);
    }
}
