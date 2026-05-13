using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin/staff")]
[ApiController]
public class AdminStaffController : ControllerBase
{
    private readonly IAuthService _authService;

    public AdminStaffController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterStaff(RegisterDTO model)
    {
        model.UserRole = "Staff";
        var result = await _authService.RegisterAsync(model);

        if (result.Succeeded)
            return Ok(new { message = "Staff registered successfully." });

        return BadRequest(result.Errors);
    }
}
