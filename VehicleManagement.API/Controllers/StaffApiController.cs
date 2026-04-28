using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[Authorize(Roles = "Staff")]
[Route("api/staff")]
[ApiController]
public class StaffApiController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffApiController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpPost("customers")]
    public async Task<IActionResult> RegisterCustomerWithVehicle(StaffRegisterCustomerDto model)
    {
        var (result, data) = await _staffService.RegisterCustomerWithVehicleAsync(model);
        if (result.Succeeded && data != null)
            return Ok(data);

        return BadRequest(result.Errors);
    }
}
