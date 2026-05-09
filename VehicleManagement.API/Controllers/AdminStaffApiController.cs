using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[ApiController]
[Route("api/admin/staff")]
[Authorize(Roles = "Admin")]
public class AdminStaffApiController : ControllerBase
{
    private readonly IAdminStaffService _svc;

    public AdminStaffApiController(IAdminStaffService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int take = 100)
        => Ok(await _svc.GetStaffAsync(take));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdminCreateStaffDto dto)
    {
        var (ok, msg) = await _svc.CreateStaffAsync(dto);
        return ok ? Ok(new { message = msg }) : BadRequest(new { message = msg });
    }
}

