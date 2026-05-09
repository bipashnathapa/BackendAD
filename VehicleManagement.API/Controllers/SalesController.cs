using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[ApiController]
[Route("api/staff/sales")]
[Authorize(Roles = "Staff,Admin")]
public class SalesController : ControllerBase
{
    private readonly ISaleInvoiceService _svc;
    public SalesController(ISaleInvoiceService svc) => _svc = svc;

    [HttpPost("invoices")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateSaleInvoiceDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var inv = await _svc.CreateAsync(userId, dto);
        return inv == null
            ? BadRequest(new { message = "Could not create invoice. Check customer, parts, and stock." })
            : CreatedAtAction(nameof(GetInvoice), new { id = inv.InvoiceID }, inv);
    }

    [HttpGet("invoices/{id:int}")]
    public async Task<IActionResult> GetInvoice(int id)
    {
        var inv = await _svc.GetByIdAsync(id);
        return inv == null ? NotFound() : Ok(inv);
    }
}
