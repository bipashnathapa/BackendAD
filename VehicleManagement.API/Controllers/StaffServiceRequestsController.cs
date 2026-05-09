using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[ApiController]
[Route("api/staff/service-requests")]
[Authorize(Roles = "Staff,Admin")]
public class StaffServiceRequestsController : ControllerBase
{
    private readonly IStaffServiceRequestsService _svc;

    public StaffServiceRequestsController(IStaffServiceRequestsService svc)
    {
        _svc = svc;
    }

    [HttpGet("appointments")]
    public async Task<IActionResult> GetAppointments([FromQuery] string? status = null, [FromQuery] int take = 50)
        => Ok(await _svc.GetAppointmentsAsync(status, take));

    [HttpPut("appointments/{id:int}/status")]
    public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateServiceStatusDto dto)
    {
        var ok = await _svc.UpdateAppointmentStatusAsync(id, dto.Status);
        return ok ? Ok(new { message = "Status updated." }) : BadRequest(new { message = "Invalid appointment or status." });
    }

    [HttpGet("parts")]
    public async Task<IActionResult> GetPartRequests([FromQuery] string? status = null, [FromQuery] int take = 50)
        => Ok(await _svc.GetPartRequestsAsync(status, take));

    [HttpPut("parts/{id:int}/status")]
    public async Task<IActionResult> UpdatePartStatus(int id, [FromBody] UpdateServiceStatusDto dto)
    {
        var ok = await _svc.UpdatePartRequestStatusAsync(id, dto.Status);
        return ok ? Ok(new { message = "Status updated." }) : BadRequest(new { message = "Invalid request or status." });
    }

    [HttpGet("reviews")]
    public async Task<IActionResult> GetReviews([FromQuery] int take = 30)
        => Ok(await _svc.GetServiceReviewsAsync(take));
}

