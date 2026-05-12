using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[ApiController]
[Route("api/staff/reports")]
[Authorize(Roles = "Staff,Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _svc;
    public ReportsController(IReportService svc) => _svc = svc;

    // /api/staff/reports/regulars?minVisits=3&days=180
    [HttpGet("regulars")]
    public async Task<IActionResult> Regulars([FromQuery] int minVisits = 3, [FromQuery] int days = 180)
        => Ok(await _svc.GetRegularsAsync(minVisits, days));

    // /api/staff/reports/high-spenders?top=10
    [HttpGet("high-spenders")]
    public async Task<IActionResult> HighSpenders([FromQuery] int top = 10)
        => Ok(await _svc.GetHighSpendersAsync(top));

    // /api/staff/reports/pending-credits?olderThanDays=30
    [HttpGet("pending-credits")]
    public async Task<IActionResult> PendingCredits([FromQuery] int olderThanDays = 30)
        => Ok(await _svc.GetPendingCreditsAsync(olderThanDays));
}
