using Microsoft.AspNetCore.Authorization; // Add this!
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[Authorize] 
[Route("api/[controller]")]
[ApiController]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddVehicle(VehicleDTO model)
    {
        var result = await _vehicleService.RegisterVehicleAsync(model);
        return result ? Ok(new { message = "Added via Service Layer!" }) : BadRequest();
    }
}